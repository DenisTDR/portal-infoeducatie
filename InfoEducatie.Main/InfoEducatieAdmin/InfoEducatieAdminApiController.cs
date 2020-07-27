using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using InfoEducatie.Contest.Participants.Participant;
using MCMS.Auth;
using MCMS.Base.Attributes;
using MCMS.Controllers.Api;
using MCMS.Data;
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
            var filesRepo = ServiceProvider.GetService<IRepository<FileEntity>>();
            var contestantsFile = await filesRepo.GetOneOrThrow(model.ContestantsFile.Id);
            var projectsFile = await filesRepo.GetOneOrThrow(model.ProjectsFile.Id);

            var result = await ServiceProvider.GetService<ImportService>()
                .Import(projectsFile, contestantsFile, model.JustProcessAndDebug);

            if (!model.JustProcessAndDebug)
            {
                await filesRepo.Delete(contestantsFile);
                await filesRepo.Delete(projectsFile);
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> SendActivationMails()
        {
            var participantsRepo = ServiceProvider.GetService<IRepository<ParticipantEntity>>();
            var authService = ServiceProvider.GetService<AuthService>();
            participantsRepo.ChainQueryable(q => q.Include(p => p.User));
            var participants = await participantsRepo.GetAll(p => !p.ActivationEmailSent);
            foreach (var participantEntity in participants)
            {
                await authService.SendActivationEmail(participantEntity.User, Url, Request.Scheme, "InfoEducație 2020 - Important", EmailBody);
                participantEntity.ActivationEmailSent = true;
                await participantsRepo.SaveChanges();
            }

            return Ok();
        }

        private static readonly string EmailBody = @"<p>Salut, {{fullName}}!</p>
<p>Primești acest email pentru că te-ai &icirc;nscris la ediția 2020, etapa națională, a <strong>InfoEducație</strong>.</p>
<p>Informațiile despre desfășurare le poți găsi pe Portalul concursului (program, seminarii, etc).</p>
<p>Pentru a avea acces la portal trebuie să-ți activezi contul folosind acest <a href='{{activationUrl}}' target='_blank'>link</a> (care a fost generat be baza adresei de e-mail cu care te-ai &icirc;nscris).</p>
<p>De asemenea, este foarte important să intri pe serverul de <a href='https://discord.gg/Fn5C4Wy'><strong>Discord</strong> al concursului</a>, unde se vor desfășura discuțiile și prezentarea/jurizarea efectivă a proiectelor.</p>
<p>Prima activitate este discuția cu comisia și va avea loc în această seară (luni, 27 iulie). Poți vedea mai multe detalii pe portal.</p>
<p>Mulțumim,<br />
Echipa <strong>InfoEducație</strong></p>
";
    }
}