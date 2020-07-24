using System.Collections.Generic;
using System.Threading.Tasks;
using InfoEducatie.Contest.Judging.JudgingCriteria.JudgingCriteriaSection;
using InfoEducatie.Contest.Participants.Participant;
using InfoEducatie.Contest.Participants.Project;
using MCMS.Auth;
using MCMS.Base.Attributes;
using MCMS.Base.Auth;
using MCMS.Controllers.Ui;
using MCMS.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InfoEducatie.Main.InfoEducatieAdmin
{
    [Authorize(Roles = "Admin")]
    public class InfoEducatieAdminController : AdminUiController
    {
        public override Task<IActionResult> Index()
        {
            return Task.FromResult(View() as IActionResult);
        }

        public IActionResult Import()
        {
            return View();
        }

        [HttpGet]
        public IActionResult DeleteParticipants()
        {
            return View();
        }

        [HttpPost, ActionName("DeleteParticipants")]
        public async Task<IActionResult> DeleteParticipantsConfirmed()
        {
            var participantsRepo = ServiceProvider.GetService<IRepository<ParticipantEntity>>();
            var projectsRepo = ServiceProvider.GetService<IRepository<ProjectEntity>>();
            await participantsRepo.Delete(p => true);
            await projectsRepo.Delete(p => true);


            var uService = ServiceProvider.GetService<IRepository<User>>();
            var userManager = ServiceProvider.GetService<UserManager<User>>();
            var users = await userManager.GetUsersInRoleAsync("Participant");
            uService.DbSet.RemoveRange(users);
            await uService.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> CheckCriteriaIntegrity()
        {
            var sectionsRepo = ServiceProvider.GetService<IRepository<JudgingCriteriaSectionEntity>>();
            sectionsRepo.ChainQueryable(q => q
                .Include(s => s.Category)
                .Include(s => s.Criteria).ThenInclude(c => c.Category));
            var sections = await sectionsRepo.GetAll();

            var errors = new List<string>();
            foreach (var section in sections)
            {
                foreach (var criterion in section.Criteria)
                {
                    if (criterion.Category.Id != section.Category.Id)
                    {
                        errors.Add(
                            $"Criterion '{criterion.Name}' is in section '{section.Name}' from category '{section.Category.Name}' but it's category is '{criterion.Category.Name}'");
                    }

                    if (criterion.Type != section.Type)
                    {
                        errors.Add(
                            $"Criterion '{criterion.Name}' is for type '{criterion.Type}', but it's section '{section.Name}' is for type '{section.Type}'.");
                    }
                }
            }

            return View(errors);
        }

        [ViewLayout("_ModalLayout")]
        public async Task<IActionResult> SendActivationMails()
        {
            var accountsWithoutEmailSent = await ServiceProvider.GetService<IRepository<ParticipantEntity>>().Queryable
                .CountAsync(p => !p.ActivationEmailSent);
            return View(accountsWithoutEmailSent);
        }
    }
}