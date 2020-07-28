using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfoEducatie.Contest.Categories;
using InfoEducatie.Contest.Judging.Judges;
using InfoEducatie.Contest.Judging.Judging;
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
        protected JudgingService JudgingService => ServiceProvider.GetRequiredService<JudgingService>();

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

        [Route("{categoryId?}")]
        public async Task<IActionResult> DetailedResults([FromRoute] string categoryId)
        {
            var cats = await GetAvailableCategories();
            categoryId ??= (await GetJudgeProfileOrThrow()).Category.Id;
            var model = new DetailedResultsModel {AvailableCategories = cats};

            var targetCategory = cats.Count == 1 ? cats.First() : cats.First(c => c.Id == categoryId);

            var judges = await JudgesRepo.GetAll(j => j.Category == targetCategory);

            model.JudgingPageModels = new List<JudgingPageModel>();

            foreach (var judge in judges)
            {
                model.JudgingPageModels.Add(await JudgingService.BuildJudgingPageModel(judge, JudgingType.Project));
            }

            return View(model);
        }

        private async Task<CategoryResultsModel> BuildResultsForCategory(CategoryEntity category)
        {
            var results = new CategoryResultsModel
            {
                CategoryName = category.Name,
                CategorySlug = category.Slug,
                ProjectJudgeCount =
                    await JudgesRepo.Queryable.CountAsync(j =>
                        j.Category == category &&
                        (j.AvailableFor == JudgeType.Project || j.AvailableFor == JudgeType.Both)),
                OpenJudgeCount = await JudgesRepo.Queryable.CountAsync(j =>
                    j.Category == category && (j.AvailableFor == JudgeType.Open || j.AvailableFor == JudgeType.Both))
            };

            var projects = await GetProjectsPointsTypeForCategory(category.Id, JudgingType.Project);
            var openProjects = await GetProjectsPointsTypeForCategory(category.Id, JudgingType.Open);
            foreach (var proj in projects)
            {
                proj.TotalOpenPoints =
                    openProjects.FirstOrDefault(op => op.ProjectId == proj.ProjectId)?.TotalProjectPoints ?? 0;
            }

            results.Projects = projects.OrderByDescending(p => p.TotalPoints).ToList();

            return results;
        }

        private async Task<List<ProjectResultsModel>> GetProjectsPointsTypeForCategory(string categoryId,
            JudgingType type)
        {
            return await (
                from project in ProjectsRepo.DbSet.Where(p => p.Category.Id == categoryId)
                join points in PointsRepo.DbSet.Where(p =>
                        p.Criterion.Type == type) on project equals
                    points.Project into pointsProjects
                from pointsProject in pointsProjects.DefaultIfEmpty()
                group pointsProject by new {project.Id, project.Title}
                into grouped
                select new ProjectResultsModel
                {
                    TotalProjectPoints = grouped.Sum(p => p.Points),
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