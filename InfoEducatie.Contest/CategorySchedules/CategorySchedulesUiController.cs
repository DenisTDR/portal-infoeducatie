using System.ComponentModel.DataAnnotations;
using MCMS.Controllers.Ui;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace InfoEducatie.Contest.CategorySchedules;
[Authorize(Roles = "Admin")]
[Display(Name = "CategorySchedules")]
public class CategorySchedulesUiController : GenericModalAdminUiController<CategoryScheduleEntity, CategoryScheduleFormModel, CategoryScheduleViewModel, CategorySchedulesAdminApiController>
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        base.OnActionExecuting(context);
        Repo.ChainQueryable(q => q.Include(c => c.Category));
    }
}