using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using InfoEducatie.Contest.Judging.JudgingCriteria.JudgingCriteriaSection;
using InfoEducatie.Contest.Participants.Participant;
using InfoEducatie.Contest.Participants.Project;
using InfoEducatie.Main.InfoEducatieAdmin.Diplomas;
using MCMS.Base.Attributes;
using MCMS.Base.Auth;
using MCMS.Base.Extensions;
using MCMS.Base.Helpers;
using MCMS.Controllers.Ui;
using MCMS.Files;
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

        public IActionResult SendCustomEmailToParticipants()
        {
            return View();
        }

        [ViewLayout("_ModalLayout")]
        public IActionResult CloneCriteriaFromProjectToOpenForCategory()
        {
            return View();
        }

        [ViewLayout("_ModalLayout")]
        public IActionResult BuildXlsxs()
        {
            return View();
        }

        [ViewLayout("_ModalLayout")]
        public IActionResult ListBuiltXlsxs()
        {
            var list = new List<LinkFileModel>();
            var dir = Env.GetOrThrow("RESULTS_PATH");

            foreach (var file in Directory.GetFiles(dir).OrderBy(f => f))
            {
                list.Add(new LinkFileModel
                {
                    FileName = Path.GetFileName(file),
                    Url = file.Replace(MFiles.PublicPath, MFiles.PublicVirtualPath),
                    IsPrize = false
                });
            }

            return View(list);
        }

        [HttpGet]
        public IActionResult DeleteParticipants()
        {
            return View();
        }

        [HttpPost, ActionName("DeleteParticipants")]
        public async Task<IActionResult> DeleteParticipantsConfirmed()
        {
            var participantsRepo = ServiceProvider.GetRepo<ParticipantEntity>();
            var projectsRepo = ServiceProvider.GetRepo<ProjectEntity>();
            await participantsRepo.Delete(p => true);
            await projectsRepo.Delete(p => true);


            var uService = ServiceProvider.GetRepo<User>();
            var userManager = ServiceProvider.GetRequiredService<UserManager<User>>();
            var users = await userManager.GetUsersInRoleAsync("Participant");
            uService.DbSet.RemoveRange(users);
            await uService.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> CheckCriteriaIntegrity()
        {
            var sectionsRepo = ServiceProvider.GetRepo<JudgingCriteriaSectionEntity>();
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
            var accountsWithoutEmailSent = await ServiceProvider.GetRepo<ParticipantEntity>().Queryable
                .CountAsync(p => !p.ActivationEmailSent);
            return View(accountsWithoutEmailSent);
        }

        [ViewLayout("_ModalLayout")]
        public IActionResult ListDiplomasDirectory([FromQuery] bool prizes = false)
        {
            var list = new List<LinkFileModel>();
            var path = Path.Combine(MFiles.PublicPath, "diplomas", prizes ? "prizes" : "participation");
            foreach (var file in Directory.GetFiles(path).OrderBy(f => f))
            {
                list.Add(new LinkFileModel
                {
                    FileName = Path.GetFileName(file),
                    Url = file.Replace(MFiles.PublicPath, MFiles.PublicVirtualPath),
                    IsPrize = prizes
                });
            }

            return View(list);
        }
    }
}