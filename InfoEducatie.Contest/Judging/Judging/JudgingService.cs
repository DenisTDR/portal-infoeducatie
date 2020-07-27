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
        protected IServiceProvider ServiceProvider;
        protected IMapper Mapper => ServiceProvider.GetService<IMapper>();
        protected IRepository<JudgeEntity> JudgesRepo => ServiceProvider.GetService<IRepository<JudgeEntity>>();
        protected IRepository<ProjectEntity> ProjectsRepo => ServiceProvider.GetService<IRepository<ProjectEntity>>();

        protected IRepository<ProjectJudgingCriterionPointsEntity> PointsRepo =>
            ServiceProvider.GetService<IRepository<ProjectJudgingCriterionPointsEntity>>();

        protected IRepository<JudgingCriterionEntity> JudgingCriteriaRepo =>
            ServiceProvider.GetService<IRepository<JudgingCriterionEntity>>();

        protected IRepository<JudgingCriteriaSectionEntity> SectionsRepo =>
            ServiceProvider.GetService<IRepository<JudgingCriteriaSectionEntity>>();

        public JudgingService(ServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        protected void PrepareQueryable()
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
            var model = new JudgingPageModel();
            if (type == JudgingType.Open)
            {
                ProjectsRepo.ChainQueryable(q => q.Where(p => p.IsInOpen));
            }

            model.Projects =
                Mapper.Map<List<ProjectViewModel>>(await ProjectsRepo.GetAll(p => p.Category == judge.Category));

            return model;
        }
    }
}