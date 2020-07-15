using System.ComponentModel;
using MCMS.Controllers.Ui;
using Microsoft.AspNetCore.Authorization;

namespace InfoEducatie.Main.Seminars
{
    [DisplayName("Seminars")]
    [Authorize(Roles = "Moderator, Admin")]
    public class SeminarsController : GenericModalAdminUiController<SeminarEntity, SeminarFormModel, SeminarViewModel,
        SeminarsAdminApiController>
    {
    }
}