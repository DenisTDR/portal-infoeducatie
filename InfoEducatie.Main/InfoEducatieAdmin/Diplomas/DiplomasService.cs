using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using InfoEducatie.Contest.Categories;
using InfoEducatie.Contest.Judging.JudgingCriteria;
using InfoEducatie.Contest.Judging.Results;
using InfoEducatie.Contest.Participants.Participant;
using MCMS.Base.Data;
using MCMS.Base.Exceptions;
using MCMS.Files;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;

namespace InfoEducatie.Main.InfoEducatieAdmin.Diplomas
{
    public class DiplomasService
    {
        private readonly IRepository<ParticipantEntity> _participantsRepo;
        private readonly IRepository<CategoryEntity> _categoriesRepo;
        private readonly ResultsService _resultsService;
        private readonly ILogger<DiplomasService> _logger;

        public DiplomasService(
            IRepository<ParticipantEntity> participantsRepo,
            IRepository<CategoryEntity> categoriesRepo,
            ResultsService resultsService,
            ILogger<DiplomasService> logger)
        {
            _participantsRepo = participantsRepo;
            _categoriesRepo = categoriesRepo;
            _resultsService = resultsService;
            _logger = logger;
        }

        public async Task MakeParticipationDiplomas()
        {
            _participantsRepo.ChainQueryable(q => q
                .Include(p => p.User)
                .Include(p => p.ProjectParticipants).ThenInclude(pp => pp.Project)
            );
            var participants = await _participantsRepo.GetAll();

            var cats = await _categoriesRepo.Queryable.Select(c => c.Id).ToListAsync();

            var projectIds = new List<string>();
            foreach (var cat in cats)
            {
                var catResults = await _resultsService.GetProjectsPointsTypeForCategory(cat, JudgingType.Project);
                projectIds.AddRange(catResults.Where(cr => cr.TotalPoints > 0).Select(cr => cr.ProjectId));
            }

            var filteredParticipants =
                participants.Where(p => p.Projects.Any(proj => projectIds.Contains(proj.Id))).ToList();

            _logger.LogWarning($"Found {filteredParticipants.Count} from a total of {participants.Count}.");

            var template = Image.FromFile(Path.Combine(MFiles.PublicPath, "diplomas/diploma_participare.png"));

            var outputPath = Path.Combine(MFiles.PublicPath, "diplomas/participare");
            if (!Directory.Exists(outputPath))
            {
                _logger.LogWarning($"Creating '{outputPath}' directory.");
                Directory.CreateDirectory(outputPath);
            }

            var jpgEncoder = ImageCodecInfo.GetImageDecoders()
                .FirstOrDefault(codec => codec.FormatID == ImageFormat.Jpeg.Guid);
            var myEncoder = Encoder.Quality;
            var myEncoderParameters = new EncoderParameters(1);
            var myEncoderParameter = new EncoderParameter(myEncoder, 50L);
            myEncoderParameters.Param[0] = myEncoderParameter;

            var now = DateTime.Now;
            _logger.LogWarning("Starting diplomas generation at: " + now.ToString("O"));
            Parallel.For(0, filteredParticipants.Count, i =>
            {
                var participant = participants[i];
                var img = template.Clone() as Image ?? throw new KnownException("Can't clone template image.");
                MakeParticipationDiploma(participant, img);

                var filePathWithoutExtension = Path.Combine(outputPath, "pd-" + participant.Id);

                var jpgPath = filePathWithoutExtension + ".jpg";
                img.Save(jpgPath, jpgEncoder, myEncoderParameters);

                var pngPath = filePathWithoutExtension + ".png";
                img.Save(pngPath, ImageFormat.Png);

                var pdfPath = filePathWithoutExtension + ".pdf";
                CreatePdfWithImage(pngPath, pdfPath);

                _logger.LogWarning($"Created {i}th diploma.");
            });

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
        }
    }
}