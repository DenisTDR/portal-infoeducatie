using System.Threading.Tasks;
using InfoEducatie.Contest.Categories;
using MCMS.Controllers.Api;
using MCMS.Data;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InfoEducatie.Contest.Participants.Project
{
    public class
        ProjectsAdminApiController : GenericAdminApiController<ProjectEntity, ProjectFormModel, ProjectViewModel>
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            Repo.ChainQueryable(q => q.Include(p => p.Category));
        }

        protected override Task PatchBeforeSaveNew(ProjectEntity e)
        {
            ServiceProvider.GetService<IRepository<CategoryEntity>>().Attach(e.Category);
            return base.PatchBeforeSaveNew(e);
        }
    }
}