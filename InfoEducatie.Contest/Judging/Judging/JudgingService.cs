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
using MCMS.Base.Data;
using MCMS.Base.Exceptions;
using MCMS.Base.Extensions;
using MCMS.Base.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InfoEducatie.Contest.Judging.Judging
{
    public class JudgingService
    {
        #region repos

        protected readonly IServiceProvider ServiceProvider;
        protected IMapper Mapper => ServiceProvider.GetRequiredService<IMapper>();
        protected IRepository<JudgeEntity> JudgesRepo => ServiceProvider.GetRepo<JudgeEntity>();
        protected IRepository<ProjectEntity> ProjectsRepo => ServiceProvider.GetRepo<ProjectEntity>();

        protected IRepository<ProjectJudgingCriterionPointsEntity> PointsRepo =>
            ServiceProvider.GetRepo<ProjectJudgingCriterionPointsEntity>();

        protected IRepository<JudgingCriterionEntity> JudgingCriteriaRepo =>
            ServiceProvider.GetRepo<JudgingCriterionEntity>();

        protected IRepository<JudgingCriteriaSectionEntity> SectionsRepo =>
            ServiceProvider.GetRepo<JudgingCriteriaSectionEntity>();

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
            ProjectsRepo.ChainQueryable(q => q.Where(p => !p.Disabled));
        }

        public async Task<JudgingPageModel> BuildJudgingPageModel(JudgeEntity judge, JudgingType type)
        {
            var model = new JudgingPageModel { Judge = judge, Type = type };

            var projectsE = await ProjectsRepo.GetAll(p =>
                p.Category == judge.Category && (type == JudgingType.Project || p.IsInOpen));

            model.Category = judge.Category;
            model.Projects = Mapper.Map<List<ProjectViewModel>>(projectsE);

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
            var criterionType = await JudgingCriteriaRepo.Query.Where(jc => jc.Id == criterionId).Select(jc => jc.Type)
                .FirstOrDefaultAsync();
            if (criterionType == JudgingType.Project && Env.GetBool("FREEZE_PROJECTS_POINTS")
                || criterionType == JudgingType.Open && Env.GetBool("FREEZE_OPEN_POINTS"))
            {
                throw new KnownException("Nu mai poți edita punctajele. Introducerea punctajelor a fost blocată.");
            }

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

            var judgingType = await JudgingCriteriaRepo.Queryable.Where(jc => jc.Id == criterionId)
                .Select(jc => jc.Type).FirstOrDefaultAsync();

            var projectPoints = await PointsRepo.DbSet.Where(p =>
                    p.Criterion.Type == judgingType && p.Judge == judge && p.Project.Id == projectId)
                .SumAsync(p => p.Points);

            var sectionsPoints = await PointsRepo.DbSet
                .Where(p => p.Criterion.Type == judgingType && p.Judge == judge && p.Project.Id == projectId)
                .GroupBy(p => p.Criterion.Section.Id)
                .Select(g => new { Id = g.Key, Points = g.Sum(p => p.Points) }).ToListAsync();

            var dict = new Dictionary<string, object>
            {
                ["total"] = projectPoints.ToString(),
                ["sections"] = sectionsPoints,
            };
            return dict;
        }
    }
}