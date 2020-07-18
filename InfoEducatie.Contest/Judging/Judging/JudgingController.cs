using System.Collections.Generic;
using System.Threading.Tasks;
using InfoEducatie.Contest.Judging.Judge;
using InfoEducatie.Contest.Judging.JudgingCriteria;
using InfoEducatie.Contest.Participants.Project;
using MCMS.Controllers.Ui;
using MCMS.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InfoEducatie.Contest.Judging.Judging
{
    [Authorize(Roles = "Jury")]
    public class JudgingController : UiController
    {
        protected IRepository<JudgeEntity> JudgesRepo => ServiceProvider.GetService<IRepository<JudgeEntity>>();
        protected IRepository<ProjectEntity> ProjectsRepo => ServiceProvider.GetService<IRepository<ProjectEntity>>();

        protected IRepository<JudgingCriterionEntity> JudgingCriteriaRepo =>
            ServiceProvider.GetService<IRepository<JudgingCriterionEntity>>();

        public async Task<IActionResult> Index()
        {
            var judgesRepo = JudgesRepo.ChainQueryable(q => q.Include(j => j.Category).Include(j => j.User));
            var model = new JudgingPageModel {Judge = await judgesRepo.GetOne(j => j.User.Id == UserFromClaims.Id)};

            model.Projects =
                Mapper.Map<List<ProjectViewModel>>(await ProjectsRepo.GetAll(p => p.Category == model.Category));
            model.JudgingCriteria =
                Mapper.Map<List<JudgingCriterionViewModel>>(
                    await JudgingCriteriaRepo.GetAll(p => p.Category == model.Category));

            model.Category = model.Judge.Category;
            return View(model);
        }
    }
}