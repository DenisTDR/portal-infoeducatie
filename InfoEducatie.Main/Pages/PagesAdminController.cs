using System.ComponentModel;
using MCMS.Controllers.Ui;

namespace InfoEducatie.Main.Pages
{
    [DisplayName("Basic pages")]
    public class PagesAdminController : GenericModalAdminUiController<PageEntity, PageFormModel, PageViewModel,
        PagesAdminApiController>
    {
    }
}