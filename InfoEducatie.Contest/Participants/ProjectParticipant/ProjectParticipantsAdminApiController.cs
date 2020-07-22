using System.Threading.Tasks;
using InfoEducatie.Contest.Participants.Participant;
using InfoEducatie.Contest.Participants.Project;
using MCMS.Controllers.Api;
using MCMS.Data;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InfoEducatie.Contest.Participants.ProjectParticipant
{
    public class ProjectParticipantsAdminApiController : GenericAdminApiController<ProjectParticipantEntity,
        ProjectParticipantFormModel, ProjectParticipantViewModel>
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            Repo.ChainQueryable(q => q.Include(pp => pp.Participant).ThenInclude(p => p.User));
            Repo.ChainQueryable(q => q.Include(pp => pp.Project).ThenInclude(p => p.Category));
        }

        protected override Task PatchBeforeSaveNew(ProjectParticipantEntity e)
        {
            ServiceProvider.GetService<IRepository<ProjectEntity>>().Attach(e.Project);
            ServiceProvider.GetService<IRepository<ParticipantEntity>>().Attach(e.Participant);
            return Task.CompletedTask;
        }
    }
}