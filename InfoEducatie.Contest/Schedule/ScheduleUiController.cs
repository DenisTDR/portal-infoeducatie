using System.Threading.Tasks;
using MCMS.Controllers.Ui;
using Microsoft.AspNetCore.Mvc;

namespace InfoEducatie.Contest.Schedule;

public class ScheduleUiController : AdminUiController
{
    public async Task<IActionResult> Generate()
    {
        var config = await ScheduleService.GetConfig();
        var schedule = await Service<ScheduleService>().Generate(config);
        return View(schedule);
    }
}