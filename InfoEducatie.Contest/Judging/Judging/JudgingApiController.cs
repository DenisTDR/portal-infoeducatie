using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using MCMS.Base.Attributes;
using MCMS.Controllers.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InfoEducatie.Contest.Judging.Judging;

[Authorize(Roles = "Jury")]
public class JudgingApiController : AdminApiController
{
    private JudgingService JudgingService => Service<JudgingService>();

    private JudgeProfileProviderService JudgeProfileProviderService => Service<JudgeProfileProviderService>();

    [HttpPost]
    [ModelValidation]
    public async Task<ActionResult<Dictionary<string, object>>> SetPoints(
        [FromBody] [Required] SetPointsModel model)
    {
        var judge = await JudgeProfileProviderService.GetProfile();
        var result = await JudgingService.SetPoints(judge, model.CriterionId, model.ProjectId, model.Points);
        return Ok(result);
    }

    [HttpPost]
    [ModelValidation]
    public async Task<IActionResult> ImpersonateJudge(string judgeId)
    {
        await JudgeProfileProviderService.Impersonate(judgeId);
        return Ok();
    }

    [HttpPost]
    [ModelValidation]
    public IActionResult EndImpersonation()
    {
        JudgeProfileProviderService.EndImpersonation();
        return Ok();
    }
}