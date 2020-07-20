using System.Threading.Tasks;
using InfoEducatie.Contest.Participants.Participant;
using InfoEducatie.Contest.Participants.Project;
using MCMS.Base.Auth;
using MCMS.Controllers.Ui;
using MCMS.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
    }
}