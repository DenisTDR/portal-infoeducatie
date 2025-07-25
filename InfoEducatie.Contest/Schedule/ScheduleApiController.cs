using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using InfoEducatie.Contest.Schedule.Models;
using MCMS.Base.Attributes;
using MCMS.Controllers.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Razor.Templating.Core;

namespace InfoEducatie.Contest.Schedule;

[Authorize(Roles = "Admin")]
[ApiExplorerSettings(GroupName = "admin-api")]
public class ScheduleApiController : AdminApiController
{
    private ScheduleService ScheduleService => Service<ScheduleService>();

    [HttpPost]
    [ModelValidation]
    public async Task<ActionResult> SaveScheduleConfig([Required] [FromBody] ScheduleConfigModel config)
    {
        await ScheduleService.PersistConfig(config);
        return Ok();
    }

    [HttpGet]
    public async Task<ActionResult<ScheduleConfigModel>> GetScheduleConfig()
    {
        var config = await ScheduleService.GetConfig();
        return Ok(config);
    }

    [HttpPost]
    [ModelValidation]
    public async Task<ActionResult<ScheduleDto>> GenerateSchedule()
    {
        var config = await ScheduleService.GetConfig();
        var dto = await ScheduleService.Generate(config);
        return Ok(dto);
    }

    [HttpPost]
    [ModelValidation]
    public async Task<ActionResult<ScheduleDto>> GenerateScheduleWithConfig(
        [Required] [FromBody] ScheduleConfigModel config)
    {
        var dto = await ScheduleService.Generate(config);
        return Ok(dto);
    }

    [HttpPost]
    [ModelValidation]
    public async Task<ActionResult<ScheduleDto>> GenerateScheduleAsHtml()
    {
        var config = await ScheduleService.GetConfig();
        var dto = await ScheduleService.Generate(config);
        var html = await RazorTemplateEngine.RenderPartialAsync("SchedulePartial", dto);
        return Content(html, "text/html");
    }

    [HttpGet]
    public async Task<ActionResult<ScheduleConfigModel>> GetScheduleXlsx()
    {
        var config = await Service<ScheduleService>().GetFromDb();

        var workbook = Service<ScheduleXlsxService>().BuildXlsx(config);
        var ms = new MemoryStream();
        workbook.SaveAs(ms);
        ms.Position = 0;

        return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "program-infoeducatie-2025.xlsx");
    }
}