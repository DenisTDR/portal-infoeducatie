using MCMS.Controllers.Api;
using Microsoft.AspNetCore.Authorization;

namespace InfoEducatie.Main.Pages
{
    [Authorize(Roles = "Moderator")]
    public class PagesAdminApiController : CrudAdminApiController<PageEntity, PageFormModel, PageViewModel>
    {
    }
}