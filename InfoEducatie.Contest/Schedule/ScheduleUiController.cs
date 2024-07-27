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
        var config = new ScheduleDto { Categories = [] };
        var currentEdition = await Repo.Query.OrderByDescending(cs => cs.Edition).Select(cs => cs.Edition)
            .FirstOrDefaultAsync();
        var configs = await Repo.Query
            .Include(cs => cs.Category)
            .Where(cs => cs.Edition == currentEdition)
            .OrderBy(cs => cs.Category.Name)
            .ToListAsync();

        foreach (var catEntity in configs)
        {
            config.Categories.Add(JsonConvert.DeserializeObject<ScheduleCategoryDto>(catEntity.JsonData));
        }


        return View(config);
    }

    public async Task<IActionResult> Generate()
    {
        var config = await ScheduleService.GetConfig();
        var schedule = await Service<ScheduleService>().Generate(config);
        return View(schedule);
    }
}