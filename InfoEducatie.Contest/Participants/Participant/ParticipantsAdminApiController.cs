using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MCMS.Controllers.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace InfoEducatie.Contest.Participants.Participant
{
    public class ParticipantsAdminApiController : CrudAdminApiController<ParticipantEntity, ParticipantFormModel,
        ParticipantViewModel>
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            Repo.ChainQueryable(q => q
                .Include(p => p.User)
                .Include(p => p.Projects)
                .ThenInclude(p => p.Category)
            );
        }

        public override async Task<ActionResult<List<ParticipantViewModel>>> IndexLight()
        {
            Repo.ChainQueryable(q => q
                .Include(p => p.User)
                .Select(e => new ParticipantEntity
                    { Id = e.Id, User = e.User }));
            var all = await Repo.GetAll();
            var allVm = Mapper.Map<List<ParticipantViewModel>>(all);
            return Ok(allVm);
        }
    }
}