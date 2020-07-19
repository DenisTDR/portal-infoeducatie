using System.Threading.Tasks;
using MCMS.Controllers.Ui;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InfoEducatie.Contest.Judging.Results
{
    [Authorize(Roles = "Moderator, Jury")]
    public class ResultsController : UiController
    {
        [Route("/[controller]")]
        public async Task<IActionResult> Index()
        {
            await Task.Delay(1000);
            return View();
        }
    }
}