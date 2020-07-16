using System.Threading.Tasks;
using InfoEducatie.Contest.Judging.Judge;
using InfoEducatie.Contest.Judging.JudgingCriteria;
using InfoEducatie.Contest.Participants.Project;
using MCMS.Controllers.Api;
using MCMS.Data;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InfoEducatie.Contest.Judging.ProjectJudgingCriterionPoints
{
    public class ProjectJudgingCriterionPointsAdminApiController : GenericAdminApiController<
        ProjectJudgingCriterionPointsEntity, ProjectJudgingCriterionPointsFormModel,
        ProjectJudgingCriterionPointsViewModel>
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            Repo.ChainQueryable(q => q
                .Include(pjcp => pjcp.Criterion)
                .Include(pjcp => pjcp.Project)
                .Include(pjcp => pjcp.Judge)
                .ThenInclude(j => j.User)
            );
        }

        protected override Task PatchBeforeSaveNew(ProjectJudgingCriterionPointsEntity e)
        {
            ServiceProvider.GetService<IRepository<ProjectEntity>>().Attach(e.Project);
            ServiceProvider.GetService<IRepository<JudgeEntity>>().Attach(e.Judge);
            ServiceProvider.GetService<IRepository<JudgingCriterionEntity>>().Attach(e.Criterion);
            return base.PatchBeforeSaveNew(e);
        }
    }
}