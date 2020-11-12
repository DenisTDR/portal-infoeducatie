using System.Threading.Tasks;
using InfoEducatie.Contest.Participants.Participant;
using InfoEducatie.Contest.Participants.Project;
using MCMS.Base.Extensions;
using MCMS.Controllers.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace InfoEducatie.Contest.Participants.ProjectParticipant
{
    [Authorize(Roles = "Admin")]
    public class ProjectParticipantsAdminApiController : GenericAdminApiController<ProjectParticipantEntity,
        ProjectParticipantFormModel, ProjectParticipantViewModel>
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            Repo.ChainQueryable(q => q.Include(pp => pp.Participant).ThenInclude(p => p.User));
            Repo.ChainQueryable(q => q.Include(pp => pp.Project).ThenInclude(p => p.Category));
        }

        protected override Task OnCreating(ProjectParticipantEntity e)
        {
            ServiceProvider.GetRepo<ProjectEntity>().Attach(e.Project);
            ServiceProvider.GetRepo<ParticipantEntity>().Attach(e.Participant);
            return Task.CompletedTask;
        }
    }
}