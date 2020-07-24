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
                await authService.SendActivationEmail(participantEntity.User, Url, Request.Scheme);
                participantEntity.ActivationEmailSent = true;
                await participantsRepo.SaveChanges();
            }

            return Ok();
        }
    }
}