using System.Threading.Tasks;
using InfoEducatie.Contest.Judging.Judges;
using InfoEducatie.Contest.Judging.JudgingCriteria;
using InfoEducatie.Contest.Participants.Project;
using MCMS.Base.Extensions;
using MCMS.Controllers.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace InfoEducatie.Contest.Judging.ProjectJudgingCriterionPoints
{
    [Authorize(Roles = "God")]
    public class ProjectJudgingCriterionPointsAdminApiController : CrudAdminApiController<
        ProjectJudgingCriterionPointsEntity, ProjectJudgingCriterionPointsFormModel,
        ProjectJudgingCriterionPointsViewModel>
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            Repo.ChainQueryable(q => q
                .Include(pjcp => pjcp.Criterion)
                .ThenInclude(crit => crit.Category)
                .Include(pjcp => pjcp.Criterion)
                .ThenInclude(crit => crit.Section)
                .Include(pjcp => pjcp.Project)
                .Include(pjcp => pjcp.Judge)
                .ThenInclude(j => j.User)
            );
        }

        protected override Task OnCreating(ProjectJudgingCriterionPointsEntity e)
        {
            ServiceProvider.GetRepo<ProjectEntity>().Attach(e.Project);
            ServiceProvider.GetRepo<JudgeEntity>().Attach(e.Judge);
            ServiceProvider.GetRepo<JudgingCriterionEntity>().Attach(e.Criterion);
            return base.OnCreating(e);
        }
    }
}