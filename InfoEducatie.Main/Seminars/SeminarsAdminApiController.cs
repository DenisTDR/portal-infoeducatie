using MCMS.Controllers.Api;
using Microsoft.AspNetCore.Authorization;

namespace InfoEducatie.Main.Seminars
{
    [Authorize(Roles = "Moderator")]
    public class
        SeminarsAdminApiController : CrudAdminApiController<SeminarEntity, SeminarFormModel, SeminarViewModel>
    {
    }
}