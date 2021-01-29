using System.ComponentModel;
using System.Threading.Tasks;
using MCMS.Controllers.Ui;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InfoEducatie.Contest.Participants.Project
{
    [DisplayName("Projects")]
    public class ProjectsAdminController : GenericModalAdminUiController<ProjectEntity, ProjectFormModel,
        ProjectViewModel, ProjectsAdminApiController>
    {
        public override Task<IActionResult> Details(string id)
        {
            Repo.ChainQueryable(q => q
                .Include(p => p.Category)
                .Include(p => p.Participants).ThenInclude(p => p.User));
            return base.Details(id);
        }
    }
}