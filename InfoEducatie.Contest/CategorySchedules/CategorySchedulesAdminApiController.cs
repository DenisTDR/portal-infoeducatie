using MCMS.Controllers.Api;
using System.Threading.Tasks;
using MCMS.Base.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using InfoEducatie.Contest.Categories;
using Microsoft.AspNetCore.Authorization;

namespace InfoEducatie.Contest.CategorySchedules;
[Authorize(Roles = "Admin")]
public class CategorySchedulesAdminApiController : CrudAdminApiController<CategoryScheduleEntity, CategoryScheduleFormModel, CategoryScheduleViewModel>
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        base.OnActionExecuting(context);
        Repo.ChainQueryable(q => q.Include(c => c.Category));
    }

    protected override Task OnCreating(CategoryScheduleEntity e)
    {
        if (e.Category != null)
            e.Category = ServiceProvider.GetRepo<CategoryEntity>().Attach(e.Category);
        return Task.CompletedTask;
    }
}