using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfoEducatie.Contest.Judging.Judging;
using InfoEducatie.Contest.Judging.JudgingCriteria;
using InfoEducatie.Contest.Judging.ProjectJudgingCriterionPoints;
using InfoEducatie.Contest.Participants.Project;
using MCMS.Base.Data;
using MCMS.Base.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InfoEducatie.Contest.Judging.Results
{
    public class ResultsService
    {
        private readonly IServiceProvider _serviceProvider;
        protected IRepository<ProjectEntity> ProjectsRepo => _serviceProvider.GetRepo<ProjectEntity>();
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
    }
}