using System.ComponentModel;
using MCMS.Controllers.Ui;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace InfoEducatie.Contest.Judging.JudgingCriteria
{
    [DisplayName("Judging Criteria")]
    [Authorize(Roles = "Moderator")]
    public class JudgingCriteriaAdminController : GenericModalAdminUiController<JudgingCriterionEntity,
        JudgingCriterionFormModel, JudgingCriterionViewModel, JudgingCriteriaAdminApiController>
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            Repo.ChainQueryable(q => q
                .Include(jc => jc.Category)
                .Include(jc => jc.Section));
        }
    }
}