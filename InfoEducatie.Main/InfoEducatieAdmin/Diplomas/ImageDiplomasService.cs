using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using InfoEducatie.Contest.Categories;
using InfoEducatie.Contest.Judging.Results;
using InfoEducatie.Contest.Participants.Participant;
using InfoEducatie.Contest.Participants.Project;
using MCMS.Base.Data;
using MCMS.Base.Exceptions;
using MCMS.Emailing.Clients.SendGrid;
using MCMS.Files;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace InfoEducatie.Main.InfoEducatieAdmin.Diplomas
{
    public class ImageDiplomasService
    {
        private readonly IRepository<ParticipantEntity> _participantsRepo;
        private readonly IRepository<ProjectEntity> _projectsRepo;
        private readonly IRepository<CategoryEntity> _categoriesRepo;
        private readonly ResultsService _resultsService;
        private readonly ILogger<ImageDiplomasService> _logger;
        private readonly MSendgridClientOptions sendgridConfig;

        public ImageDiplomasService(
            IRepository<ParticipantEntity> participantsRepo,
            IRepository<CategoryEntity> categoriesRepo,
            IRepository<ProjectEntity> projectsRepo,
            ResultsService resultsService,
            ILogger<ImageDiplomasService> logger,
            IOptions<MSendgridClientOptions> clientOptions)
        {
            _participantsRepo = participantsRepo;
            _projectsRepo = projectsRepo;
            _categoriesRepo = categoriesRepo;
            _resultsService = resultsService;
            _logger = logger;
            sendgridConfig = clientOptions.Value;
        }


        public async Task MakePrizesDiplomas()
        {
            _participantsRepo.ChainQueryable(q => q
                .Include(p => p.User)
                // .Include(p => p.ProjectParticipants).ThenInclude(pp => pp.Project)
                .Where(p => p.Projects.Any(pp => pp.ScoreProject > 0))
            );
            _projectsRepo.ChainQueryable(q => q
                .Include(p => p.Participants)
                .ThenInclude(part => part.User)
            );

            var outputPath = Path.Combine(MFiles.PublicPath, "diplomas/prizes");
            if (!Directory.Exists(outputPath))
            {
                _logger.LogWarning($"Creating '{outputPath}' directory.");
                Directory.CreateDirectory(outputPath);
            }

            var jpgEncoder = ImageCodecInfo.GetImageDecoders()
                .FirstOrDefault(codec => codec.FormatID == ImageFormat.Jpeg.Guid);
            var jpgEncoderParams = new EncoderParameters(1)
                {Param = {[0] = new EncoderParameter(Encoder.Quality, 75)}};
            var template = Image.FromFile(Path.Combine(MFiles.PrivatePath, "diplomas/diploma_premii.png"));

            var cats = await _categoriesRepo.GetAll();
            foreach (var cat in cats)
            {
                _logger.LogWarning($"Generating prizes diplomas for '{cat.Name}'.");
                var projects = await _projectsRepo.Queryable.Where(p => p.Category == cat)
                    .OrderByDescending(p => p.ScoreProject + p.ScoreOpen).Take(5).ToListAsync();
                for (int i = 0; i < projects.Count; i++)
                {
                    var project = projects[i];
                    var prize = i < 3 ? new String('I', i + 1) : "M";
                    foreach (var participant in project.Participants)
                    {
                        Console.WriteLine(prize + " - " + participant.User.FullName);
                        var img = template.Clone() as Image ?? throw new KnownException("Can't clone image template.");
                        MakePrizeDiploma(img, prize, cat.Name, participant);

                        var diplomaPath = Path.Combine(outputPath, $"{cat.Slug}-{prize}-{participant.Id}.png");
                        // img.Save(diplomaPath, ImageFormat.Jpeg);


                        var jpgPath = diplomaPath.Replace(".png", ".jpg");
                        img.Save(jpgPath, jpgEncoder, jpgEncoderParams);

                        var pdfPath = jpgPath.Replace(".jpg", ".pdf");
                        CreatePdfWithImage(jpgPath, pdfPath);
                    }
                }
            }

            _logger.LogWarning($"Generated prizes diplomas.");
        }

        public async Task MakeParticipationDiplomas()
        {
            _participantsRepo.ChainQueryable(q => q
                .Include(p => p.User)
                // .Include(p => p.ProjectParticipants).ThenInclude(pp => pp.Project)
                .Where(p => p.Projects.Any(pp => pp.ScoreProject > 0))
            );
            var participants = await _participantsRepo.GetAll();

            _logger.LogWarning(
                $"Found {participants.Count} participants with score (from a total of {await _participantsRepo.DbSet.CountAsync()}).");

            var template = Image.FromFile(Path.Combine(MFiles.PublicPath, "diplomas/diploma_participare.png"));
            var xTemplate = XImage.FromFile(Path.Combine(MFiles.PublicPath, "diplomas/diploma_participare.png"));

            var outputPath = Path.Combine(MFiles.PublicPath, "diplomas/participare");
            if (!Directory.Exists(outputPath))
            {
                _logger.LogWarning($"Creating '{outputPath}' directory.");
                Directory.CreateDirectory(outputPath);
            }

            var jpgEncoder = ImageCodecInfo.GetImageDecoders()
                .FirstOrDefault(codec => codec.FormatID == ImageFormat.Jpeg.Guid);
            var jpgEncoderParams = new EncoderParameters(1)
                {Param = {[0] = new EncoderParameter(Encoder.Quality, 50L)}};

            var now = DateTime.Now;
            _logger.LogWarning("Starting diplomas generation at: " + now.ToString("O"));

            foreach (var participant in participants)
            {
                var img = template.Clone() as Image ?? throw new KnownException("Can't clone template image.");
                MakeParticipationDiploma(participant, img);

                var filePathWithoutExtension = Path.Combine(outputPath, "pd-" + participant.Id);

                // var jpgPath = filePathWithoutExtension + ".jpg";
                // img.Save(jpgPath, jpgEncoder, jpgEncoderParams);

                var pngPath = filePathWithoutExtension + ".png";
                img.Save(pngPath, ImageFormat.Png);

                var pdfPath = filePathWithoutExtension + ".pdf";
                CreatePdfWithImage(pngPath, pdfPath);
                //
                // var pdfTextsPath = filePathWithoutExtension + "-texts.pdf";
                // CreatePdfWithTexts(xTemplate, pdfTextsPath, participant);

                _logger.LogWarning($"Created {participants.IndexOf(participant)}th diploma.");
                img.Dispose();
            }

            template.Dispose();
            xTemplate.Dispose();

            var span = DateTime.Now - now;
            _logger.LogWarning("Finished diplomas generation at: " + DateTime.Now.ToString("O") + "\nDuration: " +
                               span.ToString("g"));
        }

        private void CreatePdfWithImage(string imagePath, string pdfPath)
        {
            var img = XImage.FromFile(imagePath);
            var pdfPage = new PdfPage {Width = img.PointWidth, Height = img.PointHeight};
            var document = new PdfDocument();
            document.AddPage(pdfPage);
            var gfx = XGraphics.FromPdfPage(pdfPage);
            gfx.DrawImage(img, 0, 0);
            
            document.Save(pdfPath);
            gfx.Dispose();
            img.Dispose();
            document.Dispose();
        }

        private void CreatePdfWithTexts(XImage img, string pdfPath, ParticipantEntity participant)
        {
            var pdfPage = new PdfPage {Width = img.PointWidth, Height = img.PointHeight};
            var document = new PdfDocument();
            document.AddPage(pdfPage);
            var gfx = XGraphics.FromPdfPage(pdfPage);
            gfx.DrawImage(img, 0, 0);

            var drawFont = new XFont("Arial", 48);
            var drawBrush = new XSolidBrush(XColors.Black);

            var texts = new List<ImageTextModel>
            {
                new ImageTextModel(participant.User.FullName, 1400f, 1002f),
                new ImageTextModel(participant.School, 1100f, 1124f),
                new ImageTextModel(participant.SchoolCity, 1524f, 1244f),
                new ImageTextModel(participant.MentoringTeacher, 1706f, 1481f) {Font = new Font("Arial", 35)}
            };

            foreach (var textModel in texts)
            {
                gfx.DrawString(textModel.Text,
                    textModel.Font == null ? drawFont : new XFont("Arial", textModel.Font.Size), drawBrush, textModel.X,
                    textModel.Y);
            }

            document.Save(pdfPath);
            gfx.Dispose();
            document.Dispose();
        }

        private void MakePrizeDiploma(Image img, string prize, string category, ParticipantEntity participant)
        {
            var g = Graphics.FromImage(img);

            var drawFont = new Font("Arial", 42);
            var drawBrush = new SolidBrush(Color.Black);

            var texts = new List<ImageTextModel>
            {
                new ImageTextModel(prize, 2066, 843),
                new ImageTextModel(participant.User.LastName + " " + participant.User.FirstName, 1800, 990),
                // {Font = new Font("Arial", 42)},
                new ImageTextModel(participant.School, 1100, 1100),
                new ImageTextModel(participant.SchoolCity, 1651, 1215),
                new ImageTextModel(category, 1842, 1432),
                new ImageTextModel(participant.MentoringTeacher, 1746, 1555) {Font = new Font("Arial", 35)},
            };

            foreach (var textModel in texts)
            {
                g.DrawString(textModel.Text, textModel.Font ?? drawFont, drawBrush, textModel.X, textModel.Y);
            }

            g.Flush(FlushIntention.Flush);
            g.Dispose();
        }

        private void MakeParticipationDiploma(ParticipantEntity participant, Image img)
        {
            var g = Graphics.FromImage(img);

            var drawFont = new Font("Arial", 48);
            var drawBrush = new SolidBrush(Color.Black);

            var texts = new List<ImageTextModel>
            {
                new ImageTextModel(participant.User.FullName, 1400f, 1002f),
                new ImageTextModel(participant.School, 1100f, 1124f),
                new ImageTextModel(participant.SchoolCity, 1524f, 1244f),
                new ImageTextModel(participant.MentoringTeacher, 1706f, 1481f) {Font = new Font("Arial", 35)}
            };

            foreach (var textModel in texts)
            {
                g.DrawString(textModel.Text, textModel.Font ?? drawFont, drawBrush, textModel.X, textModel.Y);
            }

            g.Flush(FlushIntention.Flush);
            g.Dispose();
        }

        public async Task<int> SendParticipationDiplomaMails()
        {
            var diplomasPath = Path.Combine(MFiles.PublicPath, "diplomas/participare");
            _participantsRepo.ChainQueryable(q => q
                .Include(p => p.User)
                // .Include(p => p.ProjectParticipants).ThenInclude(pp => pp.Project)
                .Where(p => p.Projects.Any(pp => pp.ScoreProject > 0))
            );
            var participants = await _participantsRepo.GetAll();

            var subject = "InfoEducație - Diplomă participare";
            var message =
                "Salut {{NAME}}, <br/><br/> Atașat găsești un pdf cu diploma ta de participare. <br/><br/>Mulțumim, <br/> Echipa InfoEducație";
            int c = 0;
            foreach (var participant in participants)
            {
                _logger.LogWarning($"Sending email with SendGrid: '{subject}' to '{participant.User.Email}'");
                var client = new SendGridClient(sendgridConfig.Key);
                var msgText = message.Replace("{{NAME}}", participant.FirstName);
                var msg = new SendGridMessage
                {
                    From = new EmailAddress(sendgridConfig.DefaultSenderAddress, sendgridConfig.DefaultSenderName),
                    Subject = subject,
                    PlainTextContent = msgText,
                    HtmlContent = msgText,
                };
                msg.AddTo(new EmailAddress(participant.User.Email));
                // msg.AddTo(new EmailAddress("test@tdrs.ro"));

                var diplomaPath = Path.Combine(diplomasPath, "pd-" + participant.Id + ".pdf");
                if (!File.Exists(diplomaPath))
                {
                    _logger.LogError("Diploma not found: " + diplomaPath);
                    continue;
                }

                var bytes = await File.ReadAllBytesAsync(diplomaPath);
                var base64String = Convert.ToBase64String(bytes);

                var attach = new Attachment
                {
                    Filename = "diploma-participare.pdf", Disposition = "attachment", Type = "application/pdf",
                    Content = base64String
                };
                msg.AddAttachment(attach);

                msg.SetClickTracking(false, false);
                var response = await client.SendEmailAsync(msg);
                c++;
            }

            return c;
        }

        public async Task<int> SendPrizesDiplomaMails()
        {
            var diplomasPath = Path.Combine(MFiles.PublicPath, "diplomas/prizes");

            _participantsRepo.ChainQueryable(q => q
                .Include(p => p.User)
                // .Include(p => p.ProjectParticipants).ThenInclude(pp => pp.Project)
                .Where(p => p.Projects.Any(pp => pp.ScoreProject > 0))
            );
            _projectsRepo.ChainQueryable(q => q
                .Include(p => p.Participants)
                .ThenInclude(part => part.User)
            );

            var subject = "InfoEducație - Diplomă premiu";
            var message =
                "Salut {{NAME}}, <br/><br/> Atașat găsești diploma cu premiul în format pdf. <br/>Pentru orice nelămurire/problemă poți scrie pe Discord pe channelul #general<br/><br/>Mulțumim, <br/> Echipa InfoEducație";
            int c = 0;

            var cats = await _categoriesRepo.GetAll();
            var mailClient = new SendGridClient(sendgridConfig.Key);
            foreach (var cat in cats)
            {
                var projects = await _projectsRepo.Queryable.Where(p => p.Category == cat)
                    .OrderByDescending(p => p.ScoreProject + p.ScoreOpen).Take(5).ToListAsync();
                for (int i = 0; i < projects.Count; i++)
                {
                    var project = projects[i];
                    var prize = i < 3 ? new String('I', i + 1) : "M";
                    foreach (var participant in project.Participants)
                    {
                        // if ((participant.SentMails & SentMailsState.PrizeDiplomaEmailSent) != 0)
                        // {
                        // _logger.LogWarning($"mail diploma already send: '{subject}' to '{participant.User.Email}'");
                        // continue;
                        // }
                        _logger.LogWarning($"Sending email with SendGrid: '{subject}' to '{participant.User.Email}'");

                        var diplomaPath = Path.Combine(diplomasPath, $"{cat.Slug}-{prize}-{participant.Id}.pdf");
                        var bytes = await File.ReadAllBytesAsync(diplomaPath);
                        var base64String = Convert.ToBase64String(bytes);
                        var msgText = message.Replace("{{NAME}}", participant.FirstName);
                        var msg = new SendGridMessage
                        {
                            From = new EmailAddress(sendgridConfig.DefaultSenderAddress,
                                sendgridConfig.DefaultSenderName),
                            Subject = subject,
                            PlainTextContent = msgText,
                            HtmlContent = msgText,
                        };
                        msg.AddTo(new EmailAddress(participant.User.Email, participant.User.FullName));

                        msg.AddAttachment(new Attachment
                        {
                            Filename = "diploma-premiu.pdf", Disposition = "attachment", Type = "application/pdf",
                            Content = base64String
                        });
                        msg.SetClickTracking(true, true);
                        var response = await mailClient.SendEmailAsync(msg);
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            participant.SentMails |= SentMailsState.PrizeDiplomaEmailSent;
                            await _participantsRepo.SaveChanges();
                            c++;
                        }
                    }
                }
            }

            return c;
        }
    }
}