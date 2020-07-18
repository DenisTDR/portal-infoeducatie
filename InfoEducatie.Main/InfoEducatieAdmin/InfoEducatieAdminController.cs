using System.Threading.Tasks;
using MCMS.Controllers.Ui;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InfoEducatie.Main.InfoEducatieAdmin
{
    [Authorize(Roles = "Admin")]
    public class InfoEducatieAdminController : AdminUiController
    {
        public override Task<IActionResult> Index()
        {
            return Task.FromResult(View() as IActionResult);
        }

        public IActionResult Import()
        {
            return View();
        }
    }
}