using Microsoft.AspNetCore.Mvc;

namespace InfoEducatie.Main.Dashboard
{
    public class DashboardController: Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Admin()
        {
            return View();
        }
    }
}