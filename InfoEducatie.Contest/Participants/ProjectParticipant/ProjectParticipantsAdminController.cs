using System.ComponentModel;
using MCMS.Controllers.Ui;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace InfoEducatie.Contest.Participants.ProjectParticipant
{
    [DisplayName("Project Participants")]
    [Authorize(Roles = "Admin")]
    public class ProjectParticipantsAdminController : GenericModalAdminUiController<ProjectParticipantEntity,
        ProjectParticipantFormModel, ProjectParticipantViewModel, ProjectParticipantsAdminApiController>
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            Repo.ChainQueryable(q => q.Include(pp => pp.Participant).ThenInclude(p => p.User));
            Repo.ChainQueryable(q => q.Include(pp => pp.Project).ThenInclude(p => p.Category));
        }
    }
}