using System.ComponentModel;
using System.Threading.Tasks;
using MCMS.Controllers.Ui;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InfoEducatie.Contest.Participants.Participant
{
    [DisplayName("Participants")]
    public class ParticipantsAdminController : GenericModalAdminUiController<ParticipantEntity, ParticipantFormModel,
        ParticipantViewModel, ParticipantsAdminApiController>
    {
        public override Task<IActionResult> Details(string id)
        {
            Repo.ChainQueryable(q => q
                .Include(p => p.Project)
                .Include(p => p.User)
            );
            return base.Details(id);
        }
    }
}