using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using InfoEducatie.Contest.Judging.Judge;
using InfoEducatie.Contest.Judging.JudgingCriteria;
using InfoEducatie.Contest.Judging.ProjectJudgingCriterionPoints;
using InfoEducatie.Contest.Participants.Project;
using MCMS.Base.Attributes;
using MCMS.Base.Exceptions;
using MCMS.Controllers.Ui;
using MCMS.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

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
            PointsRepo.ChainQueryable(q => q
                .Include(p => p.Project)
                .Include(p => p.Criterion));
        }

        [Route("/[controller]")]
        public async Task<IActionResult> Index()
        {
            var model = new JudgingPageModel {Judge = await GetJudgeProfileOrThrow()};

            model.Category = model.Judge.Category;
            model.Projects =
                Mapper.Map<List<ProjectViewModel>>(await ProjectsRepo.GetAll(p => p.Category == model.Category));
            model.JudgingCriteria = Mapper.Map<List<JudgingCriterionViewModel>>(
                await JudgingCriteriaRepo.GetAll(p => p.Category == model.Category));

            model.InitialPoints = Mapper.Map<List<ProjectJudgingCriterionPointsViewModel>>(
                await PointsRepo.GetAll(p => p.Judge == model.Judge));

            return View(model);
        }

        [ApiExplorerSettings(IgnoreApi = false)]
        [HttpPost]
        [ModelValidation]
        public async Task<ActionResult<int>> SetPoints(
            [FromBody] [Required] SetPointsModel model)
        {
            var judge = await GetJudgeProfileOrThrow();
            var existing = await PointsRepo.GetOne(p =>
                p.Judge == judge && p.Criterion.Id == model.CriterionId && p.Project.Id == model.ProjectId);
            if (existing != null)
            {
                if (existing.Points != model.Points)
                {
                    existing.Points = model.Points;
                    await PointsRepo.SaveChanges();
                }
            }
            else
            {
                await PointsRepo.Add(new ProjectJudgingCriterionPointsEntity
                {
                    Points = model.Points,
                    Judge = judge,
                    Criterion = JudgingCriteriaRepo.Attach(model.CriterionId),
                    Project = ProjectsRepo.Attach(model.ProjectId),
                });
            }

            var projectPoints = await PointsRepo.DbSet.Where(p => p.Judge == judge && p.Project.Id == model.ProjectId)
                .SumAsync(p => p.Points);
            return Ok(projectPoints);
        }

        private async Task<JudgeEntity> GetJudgeProfileOrThrow()
        {
            return await JudgesRepo.GetOne(j => j.User.Id == UserFromClaims.Id) ??
                   throw new KnownException("Your account doesn't have a judge profile associated.");
        }
    }
}