using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using InfoEducatie.Contest.Judging.Judges;
using InfoEducatie.Contest.Judging.JudgingCriteria;
using InfoEducatie.Contest.Judging.ProjectJudgingCriterionPoints;
using InfoEducatie.Contest.Participants.Project;
using MCMS.Base.Attributes;
using MCMS.Base.Data;
using MCMS.Base.Exceptions;
using MCMS.Base.Extensions;
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
        protected JudgingService JudgingService => ServiceProvider.GetRequiredService<JudgingService>();

        protected IRepository<JudgeEntity> JudgesRepo => ServiceProvider.GetRepo<JudgeEntity>();
        protected IRepository<ProjectEntity> ProjectsRepo => ServiceProvider.GetRepo<ProjectEntity>();

        protected IRepository<ProjectJudgingCriterionPointsEntity> PointsRepo =>
            ServiceProvider.GetRepo<ProjectJudgingCriterionPointsEntity>();

        protected IRepository<JudgingCriterionEntity> JudgingCriteriaRepo =>
            ServiceProvider.GetRepo<JudgingCriterionEntity>();

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
            var model = await JudgingService.BuildJudgingPageModel(await GetJudgeProfileOrThrow(), type);
            return View(model);
        }

        [ApiExplorerSettings(IgnoreApi = false)]
        [HttpPost]
        [ModelValidation]
        public async Task<ActionResult<Dictionary<string, object>>> SetPoints(
            [FromBody] [Required] SetPointsModel model)
        {
            var judge = await GetJudgeProfileOrThrow();
            var result = await JudgingService.SetPoints(judge, model.CriterionId, model.ProjectId, model.Points);
            return Ok(result);
        }


        private async Task<JudgeEntity> GetJudgeProfileOrThrow()
        {
            return await JudgesRepo.GetOne(j => j.User.Id == UserFromClaims.Id) ??
                   throw new KnownException("Your account doesn't have a judge profile associated.");
        }
    }
}