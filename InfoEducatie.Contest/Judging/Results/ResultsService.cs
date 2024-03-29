using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfoEducatie.Contest.Categories;
using InfoEducatie.Contest.Judging.Judges;
using InfoEducatie.Contest.Judging.Judging;
using InfoEducatie.Contest.Judging.JudgingCriteria;
using InfoEducatie.Contest.Judging.ProjectJudgingCriterionPoints;
using InfoEducatie.Contest.Participants.Project;
using MCMS.Base.Data;
using MCMS.Base.Extensions;
using MCMS.Base.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InfoEducatie.Contest.Judging.Results
{
    public class ResultsService
    {
        private readonly IServiceProvider _serviceProvider;
        protected IRepository<ProjectEntity> ProjectsRepo => _serviceProvider.GetRepo<ProjectEntity>();
        protected IRepository<JudgeEntity> JudgesRepo => _serviceProvider.GetRepo<JudgeEntity>();
        protected JudgingService JudgingService => _serviceProvider.GetRequiredService<JudgingService>();

        protected IRepository<ProjectJudgingCriterionPointsEntity> PointsRepo =>
            _serviceProvider.GetRepo<ProjectJudgingCriterionPointsEntity>();

        public ResultsService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }


        public async Task<List<ProjectResultsModel>> GetProjectsPointsTypeForCategory(string categoryId,
            JudgingType type)
        {
            return await (
                from project in ProjectsRepo.DbSet.Where(p => p.Category.Id == categoryId && !p.Disabled)
                join points in PointsRepo.DbSet.Where(p =>
                        p.Criterion.Type == type) on project equals
                    points.Project into pointsProjects
                from pointsProject in pointsProjects.DefaultIfEmpty()
                group pointsProject by new { project.Id, project.Title }
                into grouped
                select new ProjectResultsModel
                {
                    TotalProjectPoints = grouped.Sum(p => p.Points),
                    ProjectId = grouped.Key.Id,
                    ProjectTitle = grouped.Key.Title
                }
            ).ToListAsync();
        }

        public async Task<CategoryResultsModel> GetResultsForCategory(CategoryEntity category)
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

        public async Task SaveCalculatedResults()
        {
            var catsRepo = _serviceProvider.GetRepo<CategoryEntity>();

            var cats = await catsRepo.Queryable.ToListAsync();

            var projects = await ProjectsRepo.GetAll();

            foreach (var cat in cats)
            {
                var results = await GetResultsForCategory(cat);
                foreach (var projectResultsModel in results.Projects)
                {
                    var targetProject = projects.FirstOrDefault(p => p.Id == projectResultsModel.ProjectId);
                    if (targetProject == null) continue;
                    targetProject.ScoreProject =
                        1.0f * projectResultsModel.TotalProjectPoints / results.ProjectJudgeCount /
                        (cat.ScoresX10 ? 10 : 1);
                    if (targetProject.IsInOpen)
                    {
                        targetProject.ScoreOpen =
                            1.0f * projectResultsModel.TotalOpenPoints / results.OpenJudgeCount /
                            (cat.ScoresX10 ? 10 : 1);
                    }
                    else
                    {
                        targetProject.ScoreOpen = 0;
                    }
                }

                if (!Env.GetBool("DISABLE_AUTO_PRIZER"))
                {
                    var ordered = results.Projects.OrderByDescending(p => p.TotalPoints).Take(6).ToList();
                    for (var i = 0; i < ordered.Count; i++)
                    {
                        var project = ordered[i];
                        var targetProject = projects.FirstOrDefault(p => p.Id == project.ProjectId);
                        if (targetProject == null) continue;

                        targetProject.FinalPrize = i < 3 ? new string('I', i + 1) : "M";
                    }
                }
            }

            await ProjectsRepo.SaveChanges();
        }
    }
}