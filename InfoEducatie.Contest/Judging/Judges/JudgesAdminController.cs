using System.ComponentModel;
using MCMS.Controllers.Ui;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace InfoEducatie.Contest.Judging.Judges
{
    [DisplayName("Judges")]
    [Authorize(Roles = "Moderator")]
    public class JudgesAdminController : GenericModalAdminUiController<JudgeEntity, JudgeFormModel, JudgeViewModel,
        JudgesAdminApiController>
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            Repo.ChainQueryable(q => q
                .Include(j => j.Category)
                .Include(j => j.User)
            );
        }

        public override IActionResult Create()
        {
            FormParamsService.SchemaName = nameof(JudgeCreateNewFormModel);
            return base.Create();
        }
    }
}