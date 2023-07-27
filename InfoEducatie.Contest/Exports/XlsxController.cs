using System.IO;
using System.Threading.Tasks;
using InfoEducatie.Contest.Judging.Judging;
using MCMS.Base.Exceptions;
using MCMS.Base.Helpers;
using MCMS.Controllers.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InfoEducatie.Contest.Exports;

[Authorize(Roles = "Jury, Moderator")]
[ApiExplorerSettings(GroupName = "admin-api")]
public class XlsxController : AdminApiController
{
    private JudgeProfileProviderService JudgeProfileProviderService => Service<JudgeProfileProviderService>();

    [HttpGet]
    public async Task<ActionResult> Download()
    {
        var judge = await JudgeProfileProviderService.GetProfile(true);

        if (!judge.IsVicePresident)
        {
            if (!UserFromClaims.HasRole("Admin"))
            {
                throw new KnownException("You don't have access to download din file");
            }
        }


        var dir = Env.GetOrThrow("RESULTS_PATH");
        var filePath = Path.Combine(dir, $"{judge.Category.Slug}.xlsx");

        var exportService = Service<FinalXlsxExportService>();
        var wb = await exportService.BuildWorkbookForCategory(judge.Category);
        wb.SaveAs(filePath);

        var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);

        return File(fs, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            Path.GetFileName(filePath));
    }
}