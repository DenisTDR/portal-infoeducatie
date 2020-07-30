using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using InfoEducatie.Contest.Judging.Judges;
using InfoEducatie.Contest.Judging.JudgingCriteria;
using InfoEducatie.Contest.Judging.JudgingCriteria.JudgingCriteriaSection;
using InfoEducatie.Contest.Judging.ProjectJudgingCriterionPoints;
using InfoEducatie.Contest.Participants.Project;
using MCMS.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InfoEducatie.Contest.Judging.Judging
{
    public class JudgingService
    {
        #region repos

        protected readonly IServiceProvider ServiceProvider;
        protected IMapper Mapper => ServiceProvider.GetService<IMapper>();
        protected IRepository<JudgeEntity> JudgesRepo => ServiceProvider.GetService<IRepository<JudgeEntity>>();
        protected IRepository<ProjectEntity> ProjectsRepo => ServiceProvider.GetService<IRepository<ProjectEntity>>();

        protected IRepository<ProjectJudgingCriterionPointsEntity> PointsRepo =>
            ServiceProvider.GetService<IRepository<ProjectJudgingCriterionPointsEntity>>();

        protected IRepository<JudgingCriterionEntity> JudgingCriteriaRepo =>
            ServiceProvider.GetService<IRepository<JudgingCriterionEntity>>();

        protected IRepository<JudgingCriteriaSectionEntity> SectionsRepo =>
            ServiceProvider.GetService<IRepository<JudgingCriteriaSectionEntity>>();

        #endregion

        public JudgingService(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            PrepareQueryables();
        }

        protected void PrepareQueryables()
        {
            JudgesRepo.ChainQueryable(q => q
                .Include(j => j.Category)
                .Include(j => j.User));
            PointsRepo.ChainQueryable(q => q
                .Include(p => p.Project)
                .Include(p => p.Criterion));
            SectionsRepo.ChainQueryable(q => q
                .Include(s => s.Criteria)
                .OrderBy(s => s.Name));
        }

        public async Task<JudgingPageModel> BuildJudgingPageModel(JudgeEntity judge, JudgingType type)
        {
            var model = new JudgingPageModel {Judge = judge, Type = type};
            if (type == JudgingType.Open)
            {
                ProjectsRepo.ChainQueryable(q => q.Where(p => p.IsInOpen));
            }

            model.Category = judge.Category;
            model.Projects =
                Mapper.Map<List<ProjectViewModel>>(await ProjectsRepo.GetAll(p => p.Category == judge.Category));

            var sections = await SectionsRepo.GetAll(s => s.Category == model.Category && s.Type == type);
            foreach (var section in sections)
            {
                section.Criteria = section.Criteria.OrderBy(c => c.Name).ToList();
            }

            model.JudgingSections = Mapper.Map<List<JudgingCriteriaSectionViewModel>>(sections);

            model.InitialPoints = Mapper.Map<List<ProjectJudgingCriterionPointsViewModel>>(
                await PointsRepo.GetAll(p =>
                    p.Judge == model.Judge && p.Criterion.Type == type));


            return model;
        }

        public async Task<Dictionary<string, object>> SetPoints(JudgeEntity judge, string criterionId, string projectId,
            int points)
        {
            var existing = await PointsRepo.GetOne(p =>
                p.Judge == judge && p.Criterion.Id == criterionId && p.Project.Id == projectId);
            if (existing != null)
            {
                if (existing.Points != points)
                {
                    existing.Points = points;
                    await PointsRepo.SaveChanges();
                }
            }
            else
            {
                await PointsRepo.Add(new ProjectJudgingCriterionPointsEntity
                {
                    Points = points,
                    Judge = judge,
                    Criterion = JudgingCriteriaRepo.Attach(criterionId),
                    Project = ProjectsRepo.Attach(projectId),
                });
            }

            var projectPoints = await PointsRepo.DbSet.Where(p => p.Judge == judge && p.Project.Id == projectId)
                .SumAsync(p => p.Points);

            var sectionsPoints = await PointsRepo.DbSet
                .Where(p => p.Judge == judge && p.Project.Id == projectId)
                .GroupBy(p => p.Criterion.Section.Id)
                .Select(g => new {Id = g.Key, Points = g.Sum(p => p.Points)}).ToListAsync();

            var dict = new Dictionary<string, object>
            {
                ["total"] = projectPoints.ToString(),
                ["sections"] = sectionsPoints,
            };
            return dict;
        }
    }
}