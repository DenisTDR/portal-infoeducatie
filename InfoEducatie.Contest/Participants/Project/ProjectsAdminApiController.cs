using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfoEducatie.Contest.Categories;
using MCMS.Controllers.Api;
using MCMS.Data;
using Microsoft.AspNetCore.Mvc;
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
            Repo.ChainQueryable(q => q
                .Include(p => p.Category)
                .Include(p => p.ProjectParticipants).ThenInclude(pp => pp.Participant).ThenInclude(p => p.User));
        }

        protected override Task PatchBeforeSaveNew(ProjectEntity e)
        {
            ServiceProvider.GetService<IRepository<CategoryEntity>>().Attach(e.Category);
            return base.PatchBeforeSaveNew(e);
        }

        public override Task<ActionResult<List<ProjectViewModel>>> IndexLight()
        {
            Repo.ChainQueryable(q => q.Select(e => new ProjectEntity {Id = e.Id, Title = e.Title}));
            return base.Index();
        }
    }
}