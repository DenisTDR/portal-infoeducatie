using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfoEducatie.Contest.CategorySchedules;
using InfoEducatie.Contest.Schedule.Models;
using MCMS.Base.Data;
using MCMS.Controllers.Ui;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace InfoEducatie.Contest.Schedule;

public class ScheduleUiController : AdminUiController
{
    private IRepository<CategoryScheduleEntity> Repo => Repo<CategoryScheduleEntity>();

    [AllowAnonymous]
    public override async Task<IActionResult> Index()
    {
        var config = await Service<ScheduleService>().GetFromDb();

        return View(config);
    }

    public async Task<IActionResult> Generate()
    {
        var config = await ScheduleService.GetConfig();
        var schedule = await Service<ScheduleService>().Generate(config);
        return View(schedule);
    }
}