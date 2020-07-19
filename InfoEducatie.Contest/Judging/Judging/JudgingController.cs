using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfoEducatie.Contest.Judging.Judge;
using InfoEducatie.Contest.Judging.JudgingCriteria;
using InfoEducatie.Contest.Judging.ProjectJudgingCriterionPoints;
using InfoEducatie.Contest.Participants.Project;
using MCMS.Controllers.Ui;
using MCMS.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InfoEducatie.Contest.Judging.Judging
{
    [Authorize(Roles = "Jury")]
    public class JudgingController : UiController
    {
        protected IRepository<JudgeEntity> JudgesRepo => ServiceProvider.GetService<IRepository<JudgeEntity>>();
        protected IRepository<ProjectEntity> ProjectsRepo => ServiceProvider.GetService<IRepository<ProjectEntity>>();

        protected IRepository<ProjectJudgingCriterionPointsEntity> PointsRepo =>
            ServiceProvider.GetService<IRepository<ProjectJudgingCriterionPointsEntity>>();

        protected IRepository<JudgingCriterionEntity> JudgingCriteriaRepo =>
            ServiceProvider.GetService<IRepository<JudgingCriterionEntity>>();

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            JudgesRepo.ChainQueryable(q => q.Include(j => j.Category).Include(j => j.User));
            JudgingCriteriaRepo.ChainQueryable(q => q.OrderBy(c => c.Name));
            PointsRepo.ChainQueryable(q => q.Include(p => p.Project).Include(p => p.Criterion));
        }

        [Route("[controller]")]
        public async Task<IActionResult> Index()
        {
            // var judgesRepo = JudgesRepo.ChainQueryable(q => q.Include(j => j.Category).Include(j => j.User));
            var model = new JudgingPageModel {Judge = await JudgesRepo.GetOne(j => j.User.Id == UserFromClaims.Id)};

            if (model.Judge == null)
            {
                return BadRequest("Your account doesn't have a judge profile associated.");
            }

            model.Category = model.Judge.Category;
            model.Projects =
                Mapper.Map<List<ProjectViewModel>>(await ProjectsRepo.GetAll(p => p.Category == model.Category));
            model.JudgingCriteria = Mapper.Map<List<JudgingCriterionViewModel>>(
                await JudgingCriteriaRepo.GetAll(p => p.Category == model.Category));

            model.InitialPoints = Mapper.Map<List<ProjectJudgingCriterionPointsViewModel>>(
                await PointsRepo.GetAll(p => p.Judge == model.Judge));

            return View(model);
        }
    }
}