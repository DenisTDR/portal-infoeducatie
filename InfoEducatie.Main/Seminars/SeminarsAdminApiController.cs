using MCMS.Controllers.Api;
using Microsoft.AspNetCore.Authorization;

namespace InfoEducatie.Main.Seminars
{
    [Authorize(Roles = "Moderator, Admin")]
    public class
        SeminarsAdminApiController : GenericAdminApiController<SeminarEntity, SeminarFormModel, SeminarViewModel>
    {
    }
}