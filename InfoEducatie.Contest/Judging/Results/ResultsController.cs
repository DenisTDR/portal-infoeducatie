using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using InfoEducatie.Contest.Categories;
using InfoEducatie.Contest.Exports;
using InfoEducatie.Contest.Judging.Judges;
using InfoEducatie.Contest.Judging.Judging;
using InfoEducatie.Contest.Judging.JudgingCriteria;
using InfoEducatie.Contest.Judging.ProjectJudgingCriterionPoints;
using InfoEducatie.Contest.Participants.Project;
using MCMS.Base.Data;
using MCMS.Base.Exceptions;
using MCMS.Base.Extensions;
using MCMS.Controllers.Ui;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InfoEducatie.Contest.Judging.Results
{
    [Authorize(Roles = "Moderator, Jury")]
    public class ResultsController : AdminUiController
    {
        protected IRepository<CategoryEntity> CatsRepo => ServiceProvider.GetRepo<CategoryEntity>();
        protected IRepository<JudgeEntity> JudgesRepo => ServiceProvider.GetRepo<JudgeEntity>();
        protected IRepository<ProjectEntity> ProjectsRepo => ServiceProvider.GetRepo<ProjectEntity>();
        protected JudgingService JudgingService => ServiceProvider.GetRequiredService<JudgingService>();
        protected ResultsService ResultsService => ServiceProvider.GetRequiredService<ResultsService>();

        protected IRepository<ProjectJudgingCriterionPointsEntity> PointsRepo =>
            ServiceProvider.GetRepo<ProjectJudgingCriterionPointsEntity>();

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            JudgesRepo.ChainQueryable(q => q.Include(j => j.Category).Include(j => j.User));
        }

        public override async Task<IActionResult> Index()
        {
            var results = new List<CategoryResultsModel>();
            var cats = await GetAvailableCategories();
            foreach (var categoryEntity in cats)
            {
                results.Add(await ServiceProvider.GetRequiredService<ResultsService>()
                    .GetResultsForCategory(categoryEntity));
            }

            return View(results);
        }

        [Route("{categoryId?}")]
        public async Task<IActionResult> DetailedResults([FromRoute] string categoryId)
        {
            var cats = await GetAvailableCategories();
            categoryId ??= (await GetJudgeProfileOrThrow()).Category.Id;
            var model = new DetailedResultsModel {AvailableCategories = cats};

            var targetCategory = cats.Count == 1 ? cats.First() : cats.First(c => c.Id == categoryId);

            var judges = await JudgesRepo.GetAll(j => j.Category == targetCategory);

            model.JudgingPageModels = new List<JudgingPageModel>();

            foreach (var judge in judges)
            {
                if (judge.AvailableFor.JudgesProject())
                {
                    model.JudgingPageModels.Add(await JudgingService.BuildJudgingPageModel(judge, JudgingType.Project));
                }

                if (judge.AvailableFor.JudgesOpen())
                {
                    model.JudgingPageModels.Add(await JudgingService.BuildJudgingPageModel(judge, JudgingType.Open));
                }
            }

            return View(model);
        }

        [Route("{category}/{judgeId}")]
        [Authorize(Roles = "Moderator")]
        [HttpGet]
        public async Task<IActionResult> ExportAverageScoresExceptJudgeForCategory(
            [FromRoute] [Required] string category, [FromRoute] [Required] string judgeId)
        {
            var cat = await CatsRepo.GetOneOrThrow(category);
            var judges = await JudgesRepo.GetAll(j => j.Category == cat);
            var thejudge = judges.FirstOrDefault(j => j.Id == judgeId);
            if (thejudge == null)
            {
                throw new KnownException("There is no judge with this id at this category.");
            }

            var projectJudgingPageModel = await JudgingService.BuildJudgingPageModel(thejudge, JudgingType.Project);
            var openJudgingPageModel = await JudgingService.BuildJudgingPageModel(thejudge, JudgingType.Open);
            var allPoints = await PointsRepo.GetAll(p => p.Criterion.Category == cat);

            var workbook = new XLWorkbook();
            workbook.Style.Font.FontName = "Times new roman";
            var sw = ServiceProvider.GetService<FinalXlsxExportService>();
            var sheetName = "Proj avg except " + thejudge.FullName;
            sheetName = sheetName.Substring(0, Math.Min(31, sheetName.Length));
            sw.BuildAvgScoresExceptJudgeSheet(workbook.AddWorksheet(sheetName),
                projectJudgingPageModel, allPoints, judges.Count);
            sheetName = "Open avg except " + thejudge.FullName;
            sheetName = sheetName.Substring(0, Math.Min(31, sheetName.Length));
            sw.BuildAvgScoresExceptJudgeSheet(workbook.AddWorksheet(sheetName),
                openJudgingPageModel, allPoints, judges.Count);

            var ms = new MemoryStream();
            workbook.SaveAs(ms);
            ms.Position = 0;

            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "avg-except-judge.xlsx");
        }

        private async Task<List<CategoryEntity>> GetAvailableCategories()
        {
            var isMod = UserFromClaims.HasRole("Moderator");
            List<CategoryEntity> categories;

            if (isMod)
            {
                categories = await CatsRepo.GetAll();
            }
            else
            {
                var judgeProfile = await GetJudgeProfileOrThrow();
                categories = new List<CategoryEntity> {judgeProfile.Category};
            }

            return categories;
        }

        private async Task<JudgeEntity> GetJudgeProfileOrThrow()
        {
            return await JudgesRepo.GetOne(j => j.User.Id == UserFromClaims.Id) ??
                   throw new KnownException("Your account doesn't have a judge profile associated.");
        }
    }
}