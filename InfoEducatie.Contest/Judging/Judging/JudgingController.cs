using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using InfoEducatie.Contest.Judging.JudgingCriteria;
using MCMS.Base.Attributes;
using MCMS.Controllers.Ui;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace InfoEducatie.Contest.Judging.Judging
{
    [Authorize(Roles = "Jury")]
    [ApiExplorerSettings(GroupName = "admin-api")]
    public class JudgingController : AdminUiController
    {
        private JudgingService JudgingService => ServiceProvider.GetRequiredService<JudgingService>();

        private JudgeProfileProviderService JudgeProfileProviderService =>
            ServiceProvider.GetRequiredService<JudgeProfileProviderService>();

        [NonAction]
        [ApiExplorerSettings(IgnoreApi = true)]
        public override Task<IActionResult> Index()
        {
            throw new NotImplementedException();
        }

        [AdminRoute("~/[controller]/{type?}")]
        [HttpGet]
        public async Task<IActionResult> Judging([FromRoute] [Optional] JudgingType type)
        {
            var judge = await JudgeProfileProviderService.GetProfile(true, true);
            var model = await JudgingService.BuildJudgingPageModel(judge, type);
            return View(model);
        }

        [ApiExplorerSettings(IgnoreApi = false)]
        [HttpPost]
        [ModelValidation]
        public async Task<ActionResult<Dictionary<string, object>>> SetPoints(
            [FromBody] [Required] SetPointsModel model)
        {
            var judge = await JudgeProfileProviderService.GetProfile();
            var result = await JudgingService.SetPoints(judge, model.CriterionId, model.ProjectId, model.Points);
            return Ok(result);
        }

        [ApiExplorerSettings(IgnoreApi = false)]
        [HttpPost]
        [ModelValidation]
        public async Task<IActionResult> ImpersonateJudge(string judgeId)
        {
            await JudgeProfileProviderService.Impersonate(judgeId);
            return Ok();
        }

        [ApiExplorerSettings(IgnoreApi = false)]
        [HttpPost]
        [ModelValidation]
        public IActionResult EndImpersonation()
        {
            JudgeProfileProviderService.EndImpersonation();
            return Ok();
        }
    }
}