using System.Threading.Tasks;
using InfoEducatie.Contest.Participants.Project;
using MCMS.Controllers.Api;
using MCMS.Data;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InfoEducatie.Contest.Participants.Participant
{
    public class ParticipantsAdminApiController : GenericAdminApiController<ParticipantEntity, ParticipantFormModel,
        ParticipantViewModel>
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            Repo.ChainQueryable(q => q
                .Include(p => p.Project)
                .Include(p => p.User)
            );
        }

        protected override Task PatchBeforeSaveNew(ParticipantEntity e)
        {
            ServiceProvider.GetService<IRepository<ProjectEntity>>().Attach(e.Project);
            return base.PatchBeforeSaveNew(e);
        }
    }
}