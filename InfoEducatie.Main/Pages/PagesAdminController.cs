using System.ComponentModel;
using MCMS.Controllers.Ui;
using Microsoft.AspNetCore.Authorization;

namespace InfoEducatie.Main.Pages
{
    [DisplayName("Basic pages")]
    [Authorize(Roles = "Moderator")]
    public class PagesAdminController : GenericModalAdminUiController<PageEntity, PageFormModel, PageViewModel,
        PagesAdminApiController>
    {
    }
}