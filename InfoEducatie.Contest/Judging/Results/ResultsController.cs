using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfoEducatie.Contest.Categories;
using InfoEducatie.Contest.Judging.Judge;
using InfoEducatie.Contest.Judging.JudgingCriteria;
using InfoEducatie.Contest.Judging.ProjectJudgingCriterionPoints;
using InfoEducatie.Contest.Participants.Project;
using MCMS.Base.Exceptions;
using MCMS.Controllers.Ui;
using MCMS.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InfoEducatie.Contest.Judging.Results
{
    [Authorize(Roles = "Moderator, Jury")]
    public class ResultsController : UiController
    {
        protected IRepository<CategoryEntity> CatsRepo => ServiceProvider.GetService<IRepository<CategoryEntity>>();
        protected IRepository<JudgeEntity> JudgesRepo => ServiceProvider.GetService<IRepository<JudgeEntity>>();
        protected IRepository<ProjectEntity> ProjectsRepo => ServiceProvider.GetService<IRepository<ProjectEntity>>();

        protected IRepository<ProjectJudgingCriterionPointsEntity> PointsRepo =>
            ServiceProvider.GetService<IRepository<ProjectJudgingCriterionPointsEntity>>();

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            JudgesRepo.ChainQueryable(q => q.Include(j => j.Category).Include(j => j.User));
        }

        [Route("/[controller]")]
        public async Task<IActionResult> Index()
        {
            var results = new List<CategoryResultsModel>();
            var cats = await GetAvailableCategories();
            foreach (var categoryEntity in cats)
            {
                results.Add(await BuildResultsForCategory(categoryEntity));
            }

            return View(results);
        }

        private async Task<CategoryResultsModel> BuildResultsForCategory(CategoryEntity category)
        {
            var results = new CategoryResultsModel {CategoryName = category.Name};

            var projects = await GetProjectsPointsTypeForCategory(category.Id, CriterionType.Project);
            var openProjects = await GetProjectsPointsTypeForCategory(category.Id, CriterionType.Open);
            foreach (var proj in projects)
            {
                proj.OpenPoints = openProjects.FirstOrDefault(op => op.ProjectId == proj.ProjectId)?.ProjectPoints ?? 0;
            }

            results.Projects = projects.OrderByDescending(p => p.TotalPoints).ToList();

            return results;
        }

        private async Task<List<ProjectResultsModel>> GetProjectsPointsTypeForCategory(string categoryId,
            CriterionType type)
        {
            return await (
                from project in ProjectsRepo.DbSet.Where(p => p.Category.Id == categoryId)
                join points in PointsRepo.DbSet.Where(p => p.Criterion.Type == type) on project equals
                    points.Project into pointsProjects
                from pointsProject in pointsProjects.DefaultIfEmpty()
                group pointsProject by new {project.Id, project.Title}
                into grouped
                select new ProjectResultsModel
                {
                    ProjectPoints = grouped.Sum(p => p.Points),
                    ProjectId = grouped.Key.Id,
                    ProjectTitle = grouped.Key.Title
                }
            ).ToListAsync();
        }

        private async Task<List<CategoryEntity>> GetAvailableCategories()
        {
            var isMod = UserFromClaims.HasRole("Moderator");
            List<CategoryEntity> categories;

            if (isMod)
            {
                categories = await CatsRepo.GetAll();
            }
            else
            {
                var judgeProfile = await GetJudgeProfileOrThrow();
                categories = new List<CategoryEntity> {judgeProfile.Category};
            }

            return categories;
        }

        private async Task<JudgeEntity> GetJudgeProfileOrThrow()
        {
            return await JudgesRepo.GetOne(j => j.User.Id == UserFromClaims.Id) ??
                   throw new KnownException("Your account doesn't have a judge profile associated.");
        }
    }
}