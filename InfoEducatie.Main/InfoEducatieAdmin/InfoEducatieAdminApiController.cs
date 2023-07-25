using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using InfoEducatie.Contest.Categories;
using InfoEducatie.Contest.Exports;
using InfoEducatie.Contest.Judging.JudgingCriteria;
using InfoEducatie.Contest.Judging.JudgingCriteria.JudgingCriteriaSection;
using InfoEducatie.Contest.Judging.Results;
using InfoEducatie.Contest.Participants.Participant;
using InfoEducatie.Contest.Participants.Project;
using InfoEducatie.Main.InfoEducatieAdmin.Diplomas;
using MCMS.Auth;
using MCMS.Base.Attributes;
using MCMS.Base.Extensions;
using MCMS.Base.Helpers;
using MCMS.Controllers.Api;
using MCMS.Emailing.Sender;
using MCMS.Files.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InfoEducatie.Main.InfoEducatieAdmin
{
    [Authorize(Roles = "Admin")]
    public class InfoEducatieAdminApiController : AdminApiController
    {
        [HttpPost]
        [ModelValidation]
        public async Task<IActionResult> Import([FromBody] [Required] ImportFormModel model)
        {
            var filesRepo = ServiceProvider.GetRepo<FileEntity>();
            var contestantsFile = await filesRepo.GetOneOrThrow(model.ContestantsFile.Id);
            var projectsFile = await filesRepo.GetOneOrThrow(model.ProjectsFile.Id);

            var result = await Service<ImportService>()
                .Import(projectsFile, contestantsFile, model.JustProcessAndDebug);

            if (!model.JustProcessAndDebug)
            {
                await filesRepo.Delete(contestantsFile);
                await filesRepo.Delete(projectsFile);
            }

            return Ok(result);
        }

        [HttpPost]
        [ModelValidation]
        public async Task<IActionResult> SendCustomEmailToParticipants(
            [FromBody] [Required] CustomEmailToParticipantsModel model)
        {
            if (model.Type == SendEmailToParticipantsType.Category && model.Category == null)
            {
                return BadRequest("invalid category");
            }

            var query = ServiceProvider.GetRepo<ParticipantEntity>().Queryable;
            if (model.Type == SendEmailToParticipantsType.Category)
            {
                query = query.Where(p => p.Projects.Any(pp => pp.Category.Id == model.Category.Id));
            }

            var emails = await query.Select(p => p.User.Email).ToListAsync();

            await Task.Delay(1000);
            if (!model.SendIt)
            {
                return Ok(new { debug = true, request = model, emails });
            }

            var emailSender = ServiceProvider.GetRequiredService<IMEmailSender>();

            foreach (var email in emails)
            {
                await emailSender.SendEmail("no-reply@portal.infoeducatie.ro", "InfoEducație", email, model.Subject,
                    model.Message);
            }


            return Ok(new { message = "Sent " + emails.Count + " emails!" });
        }

        [HttpPost]
        [ModelValidation]
        public async Task<IActionResult> CloneCriteriaFromProjectToOpenForCategory(
            [FromBody] [Required] CloneCriteriaFromProjectToOpenForCategoryFormModel model)
        {
            var catsRepo = ServiceProvider.GetRepo<CategoryEntity>();
            var criteriaRepo = ServiceProvider.GetRepo<JudgingCriterionEntity>();
            var sectionsRepo = ServiceProvider.GetRepo<JudgingCriteriaSectionEntity>();
            sectionsRepo.ChainQueryable(q => q.Include(s => s.Criteria));

            var category = await catsRepo.GetOneOrThrow(model.Category.Id);
            var addedSections = 0;
            var addedCriteria = 0;
            var removedSections = 0;
            var removedCriteria = 0;
            if (model.RemoveExistingOpenCriteriaFirst)
            {
                var criteriaToDelete =
                    await criteriaRepo.GetAll(s => s.Type == JudgingType.Open && s.Category.Id == model.Category.Id);
                removedCriteria = criteriaToDelete.Count;
                criteriaRepo.DbSet.RemoveRange(criteriaToDelete);

                var sectionsToDelete =
                    await sectionsRepo.GetAll(s => s.Type == JudgingType.Open && s.Category.Id == model.Category.Id);
                removedSections = sectionsToDelete.Count;
                sectionsRepo.DbSet.RemoveRange(sectionsToDelete);

                await sectionsRepo.SaveChanges();
            }

            var sections = await sectionsRepo.Queryable
                .Where(s => s.Type == JudgingType.Project && s.Category.Id == model.Category.Id).AsNoTracking()
                .ToListAsync();
            foreach (var section in sections)
            {
                section.Category = category;
                section.Type = JudgingType.Open;
                section.Id = null;
                section.Criteria.ForEach(c =>
                {
                    c.Type = JudgingType.Open;
                    c.Category = category;
                    c.Id = null;
                });
                await sectionsRepo.Add(section);
                addedSections++;
                addedCriteria += section.Criteria.Count;
            }

            return Ok(new { addedSections, addedCriteria, removedSections, removedCriteria });
        }

        [HttpPost]
        public async Task<IActionResult> SaveCalculatedResults()
        {
            await ServiceProvider.GetRequiredService<ResultsService>().SaveCalculatedResults();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> MakeParticipationDiplomas()
        {
            await ServiceProvider.GetRequiredService<PdfDiplomasService>().MakeParticipationDiplomas();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> MakePrizesDiploma()
        {
            await ServiceProvider.GetRequiredService<PdfDiplomasService>().MakePrizesDiplomas();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> SendParticipationDiplomasMails()
        {
            var c = await ServiceProvider.GetRequiredService<PdfDiplomasService>().SendParticipationDiplomaMails();
            return Ok(c);
        }

        [HttpPost]
        public async Task<IActionResult> SendPrizesDiplomaMails()
        {
            var c = await ServiceProvider.GetRequiredService<PdfDiplomasService>().SendPrizesDiplomaMails();
            return Ok(c);
        }

        [HttpPost]
        public async Task<IActionResult> PutParticipationPrize()
        {
            var updated = await ServiceProvider.Repo<ProjectEntity>().Queryable
                .Where(p => p.ScoreProject > 0 && p.FinalPrize == null)
                .UpdateFromQueryAsync(p => new ProjectEntity { FinalPrize = "PARTICIPARE" });
            return Ok(updated);
        }


        [HttpPost]
        public async Task<IActionResult> SendActivationMails()
        {
            var participantsRepo = ServiceProvider.GetRepo<ParticipantEntity>();
            var authService = ServiceProvider.GetRequiredService<AuthService>();
            participantsRepo.ChainQueryable(q => q.Include(p => p.User));
            var participants = await participantsRepo.GetAll(p => !p.ActivationEmailSent);
            foreach (var participantEntity in participants)
            {
                await authService.SendActivationEmail(participantEntity.User, Url, Request.Scheme,
                    "InfoEducație 2020 - Important", EmailBody);
                participantEntity.ActivationEmailSent = true;
                await participantsRepo.SaveChanges();
            }

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> BuildFinalShitsForAllCategories()
        {
            var origService = ServiceProvider.GetRequiredService<FinalXlsxExportService>();
            var serviceWithCeilings = ServiceProvider.GetRequiredService<FinalXlsxExportServiceWithCeilings>();

            var catsService = ServiceProvider.GetRepo<CategoryEntity>();
            var cats = await catsService.GetAll();
            var dir = Env.GetOrThrow("RESULTS_PATH");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            foreach (var categoryEntity in cats)
            {
                var wb = await origService.BuildWorkbookForCategory(categoryEntity);
                var filePath = Path.Combine(dir, categoryEntity.Slug + ".xlsx");
                Console.WriteLine("Saving to " + filePath);
                wb.SaveAs(filePath);


                // wb = await serviceWithCeilings.BuildWorkbookForCategory(categoryEntity);
                // filePath = Path.Combine(dir, categoryEntity.Slug + "-with-ceilings.xlsx");
                // Console.WriteLine("Saving to " + filePath);
                // wb.SaveAs(filePath);
            }

            return Ok();
        }

        private static readonly string EmailBody = @"<p>Salut, {{fullName}}!</p>
<p>Primești acest email pentru că te-ai &icirc;nscris la ediția 2020, etapa națională, a <strong>InfoEducație</strong>.</p>
<p>Informațiile despre desfășurare le poți găsi pe Portalul concursului (program, seminarii, etc).</p>
<p>Pentru a avea acces la portal trebuie să-ți activezi contul folosind acest <a href='{{activationUrl}}' target='_blank'>link</a> (care a fost generat be baza adresei de e-mail cu care te-ai &icirc;nscris).</p>
<p>De asemenea, este foarte important să intri pe serverul de <a href='https://discord.gg/Fn5C4Wy'><strong>Discord</strong> al concursului</a>, unde se vor desfășura discuțiile și prezentarea/jurizarea efectivă a proiectelor.</p>
<p>Mulțumim,<br />
Echipa <strong>InfoEducație</strong></p>
";

        // <p>Prima activitate este discuția cu comisia și va avea loc în această seară (luni, 27 iulie). Poți vedea mai multe detalii pe portal.</p>
    }
}