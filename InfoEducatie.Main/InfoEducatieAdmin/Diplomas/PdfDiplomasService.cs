using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using InfoEducatie.Contest.Categories;
using InfoEducatie.Contest.Participants.Participant;
using InfoEducatie.Contest.Participants.Project;
using MCMS.Base.Data;
using MCMS.Base.Helpers;
using MCMS.Emailing.Clients.SendGrid;
using MCMS.Files;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PdfSharpCore.Drawing;
using PdfSharpCore.Fonts;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace InfoEducatie.Main.InfoEducatieAdmin.Diplomas
{
    public class PdfDiplomasService
    {
        private readonly IRepository<ParticipantEntity> _participantsRepo;
        private readonly IRepository<ProjectEntity> _projectsRepo;
        private readonly IRepository<CategoryEntity> _categoriesRepo;
        private readonly ILogger<PdfDiplomasService> _logger;
        private readonly MSendgridClientOptions _sendgridConfig;

        public PdfDiplomasService(
            IRepository<ParticipantEntity> participantsRepo,
            IRepository<CategoryEntity> categoriesRepo,
            IRepository<ProjectEntity> projectsRepo,
            ILogger<PdfDiplomasService> logger,
            IOptions<MSendgridClientOptions> clientOptions)
        {
            _participantsRepo = participantsRepo;
            _projectsRepo = projectsRepo;
            _categoriesRepo = categoriesRepo;
            _logger = logger;
            _sendgridConfig = clientOptions.Value;
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
                _logger.LogWarning("Creating '{Path}' directory", outputPath);
                Directory.CreateDirectory(outputPath);
            }

            var srcPdf = PdfReader.Open(Path.Combine(MFiles.PublicPath, "diplomas/diploma-premii-2021.pdf"),
                PdfDocumentOpenMode.Import);

            if (GlobalFontSettings.FontResolver?.DefaultFontName == "Arial")
            {
                GlobalFontSettings.FontResolver = new PdfSharpFontResolver();
            }

            var cats = await _categoriesRepo.GetAll();
            var cnt = 0;
            foreach (var cat in cats)
            {
                _logger.LogWarning("Generating prizes diplomas for '{Cat}'", cat.Name);
                var projects = await _projectsRepo.Queryable.Where(p => p.Category == cat)
                    .OrderByDescending(p => p.ScoreProject + p.ScoreOpen).Take(6).ToListAsync();
                foreach (var project in projects)
                {
                    foreach (var participant in project.Participants)
                    {
                        var pdf = GetPrizeDiploma(srcPdf, project.FinalPrize, cat.Name, participant);

                        var filePath = Path.Combine(outputPath,
                            $"{cat.Slug}-{project.FinalPrize}-{participant.Id}.pdf");

                        pdf.Save(filePath);
                        pdf.Dispose();

                        _logger.LogWarning("Created {Index}th diploma", cnt);
                        cnt++;
                    }
                }
            }

            _logger.LogWarning("Generated prizes diplomas");
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
                "Found {Count} participants with score (from a total of {TotalCount})",
                participants.Count, await _participantsRepo.DbSet.CountAsync());

            var outputPath = Path.Combine(MFiles.PublicPath, "diplomas/participation");
            if (!Directory.Exists(outputPath))
            {
                _logger.LogWarning("Creating '{Path}' directory", outputPath);
                Directory.CreateDirectory(outputPath);
            }


            var srcPdf = PdfReader.Open(Path.Combine(MFiles.PublicPath, "diplomas/diploma-participare-2021.pdf"),
                PdfDocumentOpenMode.Import);
            var now = DateTime.Now;
            _logger.LogWarning("Starting diplomas generation at: {Time}", now.ToString("O"));

            if (GlobalFontSettings.FontResolver?.DefaultFontName == "Arial")
            {
                GlobalFontSettings.FontResolver = new PdfSharpFontResolver();
            }

            foreach (var participant in participants)
            {
                var pdf = GetParticipationDiploma(participant, srcPdf);

                var filePathWithoutExtension = Path.Combine(outputPath, "pd-" + participant.Id);

                var pdfPath = filePathWithoutExtension + ".pdf";

                pdf.Save(pdfPath);
                pdf.Dispose();

                _logger.LogWarning("Created {Index}th diploma", participants.IndexOf(participant));
            }

            var span = DateTime.Now - now;
            _logger.LogWarning("Finished diplomas generation at: {Time}\nDuration: {Duration}",
                DateTime.Now.ToString("O"), span.ToString("g"));
        }

        private PdfDocument GetPrizeDiploma(PdfDocument srcPdf, string prize, string category,
            ParticipantEntity participant)
        {
            var newPdf = new PdfDocument();

            foreach (var srcPdfPage in srcPdf.Pages)
            {
                newPdf.AddPage(srcPdfPage);
            }

            var page = newPdf.Pages[0];
            var gfx = XGraphics.FromPdfPage(page);
            var defaultFont = new XFont("OpenSans", 14, XFontStyle.Regular);

            var texts = new List<ImageTextModel>
            {
                new(prize, 498, 200) {XFont = new XFont("OpenSans", 16, XFontStyle.Bold)},
                new(participant.User.FullName, 435, 235),
                new(participant.School, 258, 263),
                new(participant.SchoolCity, 390, 290),
                new(category, 440, 343),
                new(participant.MentoringTeacher, 425, 371) {XFont = new XFont("OpenSans", 11, XFontStyle.Regular)},
            };

            foreach (var text in texts)
            {
                gfx.DrawString(text.Text, text.XFont ?? defaultFont, XBrushes.Black,
                    new XRect(text.X, text.Y, page.Width, page.Height),
                    new XStringFormat
                    {
                        Alignment = XStringAlignment.Near,
                        LineAlignment = XLineAlignment.Near
                    });
            }

            gfx.Dispose();
            return newPdf;
        }

        private PdfDocument GetParticipationDiploma(ParticipantEntity participant, PdfDocument srcPdf)
        {
            var newPdf = new PdfDocument();

            foreach (var srcPdfPage in srcPdf.Pages)
            {
                newPdf.AddPage(srcPdfPage);
            }

            var page = newPdf.Pages[0];
            var gfx = XGraphics.FromPdfPage(page);
            var defaultFont = new XFont("OpenSans", 14, XFontStyle.Regular);

            var texts = new List<ImageTextModel>
            {
                new(participant.User.FullName, 354, 253),
                new(participant.School, 275, 285),
                new(participant.SchoolCity, 375, 310),
                new(participant.MentoringTeacher, 425, 366) {XFont = new XFont("OpenSans", 11, XFontStyle.Regular)},
            };

            foreach (var text in texts)
            {
                gfx.DrawString(text.Text, text.XFont ?? defaultFont, XBrushes.Black,
                    new XRect(text.X, text.Y, page.Width, page.Height),
                    new XStringFormat
                    {
                        Alignment = XStringAlignment.Near,
                        LineAlignment = XLineAlignment.Near
                    });
            }

            gfx.Dispose();
            return newPdf;
        }

        private string GetRecipientEmail(string src)
        {
            return Env.GetBool("TEST_DIPLOMAS_MAILS") ? Env.Get("TEST_DIPLOMAS_RECIPIENT") : src;
        }

        public async Task<int> SendParticipationDiplomaMails()
        {
            var diplomasPath = Path.Combine(MFiles.PublicPath, "diplomas/participation");
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
                _logger.LogWarning("Sending email with SendGrid: '{Subject}' to '{Email}'", subject,
                    participant.User.Email);
                var client = new SendGridClient(_sendgridConfig.Key);
                var msgText = message.Replace("{{NAME}}", participant.FirstName);
                var msg = new SendGridMessage
                {
                    From = new EmailAddress(_sendgridConfig.DefaultSenderAddress, _sendgridConfig.DefaultSenderName),
                    Subject = subject,
                    PlainTextContent = msgText,
                    HtmlContent = msgText,
                };
                msg.AddTo(new EmailAddress(GetRecipientEmail(participant.User.Email)));
                // msg.AddTo(new EmailAddress("test@tdrs.ro"));

                var diplomaPath = Path.Combine(diplomasPath, "pd-" + participant.Id + ".pdf");
                if (!File.Exists(diplomaPath))
                {
                    _logger.LogError("Diploma not found: {Path}", diplomaPath);
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

                msg.SetClickTracking(true, true);
                await client.SendEmailAsync(msg);
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
            var mailClient = new SendGridClient(_sendgridConfig.Key);
            foreach (var cat in cats)
            {
                var projects = await _projectsRepo.Queryable.Where(p => p.Category == cat)
                    .OrderByDescending(p => p.ScoreProject + p.ScoreOpen).Take(6).ToListAsync();
                for (var i = 0; i < projects.Count; i++)
                {
                    var project = projects[i];
                    foreach (var participant in project.Participants)
                    {
                        // if ((participant.SentMails & SentMailsState.PrizeDiplomaEmailSent) != 0)
                        // {
                        // _logger.LogWarning($"mail diploma already send: '{subject}' to '{participant.User.Email}'");
                        // continue;
                        // }
                        _logger.LogWarning("Sending email with SendGrid: '{Subject}' to '{Email}'", subject,
                            participant.User.Email);

                        var diplomaPath = Path.Combine(diplomasPath,
                            $"{cat.Slug}-{project.FinalPrize}-{participant.Id}.pdf");
                        var bytes = await File.ReadAllBytesAsync(diplomaPath);
                        var base64String = Convert.ToBase64String(bytes);
                        var msgText = message.Replace("{{NAME}}", participant.FirstName);
                        var msg = new SendGridMessage
                        {
                            From = new EmailAddress(_sendgridConfig.DefaultSenderAddress,
                                _sendgridConfig.DefaultSenderName),
                            Subject = subject,
                            PlainTextContent = msgText,
                            HtmlContent = msgText,
                        };
                        msg.AddTo(
                            new EmailAddress(GetRecipientEmail(participant.User.Email), participant.User.FullName));

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