using System.Threading.Tasks;
using MCMS.Controllers.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InfoEducatie.Main.InfoEducatieAdmin.PbcakFixer;

[Authorize(Roles = "Admin")]
public class PbcakFixerController : AdminApiController
{
    private PbcakFixerService PbcakFixerService => Service<PbcakFixerService>();

    // [FromQuery] bool commit
    [HttpGet]
    public async Task<ActionResult> GetDiff()
    {
        var result = await PbcakFixerService.GetDiff();
        // return Ok(result);
        return Content(result, "text/plain");
    }
}