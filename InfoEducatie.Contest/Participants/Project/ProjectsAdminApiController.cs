using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfoEducatie.Contest.Categories;
using MCMS.Base.Extensions;
using MCMS.Controllers.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace InfoEducatie.Contest.Participants.Project
{
    public class
        ProjectsAdminApiController : CrudAdminApiController<ProjectEntity, ProjectFormModel, ProjectViewModel>
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            Repo.ChainQueryable(q => q
                .Include(p => p.Category)
                .Include(p => p.Participants).ThenInclude(p => p.User));
        }

        protected override Task OnCreating(ProjectEntity e)
        {
            ServiceProvider.GetRepo<CategoryEntity>().Attach(e.Category);
            return base.OnCreating(e);
        }

        public override Task<ActionResult<List<ProjectViewModel>>> IndexLight()
        {
            Repo.ChainQueryable(q => q.Select(e => new ProjectEntity {Id = e.Id, Title = e.Title}));
            return base.Index();
        }
    }
}