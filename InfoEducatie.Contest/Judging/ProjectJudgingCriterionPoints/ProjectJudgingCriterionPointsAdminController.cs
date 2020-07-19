using System.ComponentModel;
using MCMS.Controllers.Ui;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace InfoEducatie.Contest.Judging.ProjectJudgingCriterionPoints
{
    [DisplayName("Project Judging Criterion Points")]
    [Authorize(Roles = "Admin")]
    public class ProjectJudgingCriterionPointsAdminController : GenericModalAdminUiController<
        ProjectJudgingCriterionPointsEntity, ProjectJudgingCriterionPointsFormModel,
        ProjectJudgingCriterionPointsViewModel, ProjectJudgingCriterionPointsAdminApiController>
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
    }
}