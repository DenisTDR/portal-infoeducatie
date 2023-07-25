using System.IO;
using System.Threading.Tasks;
using InfoEducatie.Contest.Judging.Judges;
using InfoEducatie.Contest.Judging.Judging;
using MCMS.Base.Data;
using MCMS.Base.Exceptions;
using MCMS.Base.Extensions;
using MCMS.Base.Helpers;
using MCMS.Controllers.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace InfoEducatie.Contest.Exports;

[Authorize(Roles = "Jury, Moderator")]
[ApiExplorerSettings(GroupName = "admin-api")]
public class XlsxController : AdminApiController
{
    private JudgeProfileProviderService JudgeProfileProviderService => Service<JudgeProfileProviderService>();

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        base.OnActionExecuting(context);
        JudgesRepo.ChainQueryable(q => q.Include(j => j.Category));
    }

    protected IRepository<JudgeEntity> JudgesRepo => ServiceProvider.GetRepo<JudgeEntity>();

    [HttpGet]
    public async Task<ActionResult> Download()
    {
        var judge = await JudgeProfileProviderService.GetProfile();
        
        if (!judge.IsVicePresident)
        {
            if (!UserFromClaims.HasRole("Admin"))
            {
                throw new KnownException("You don't have access to download din file");
            }
        }

        var dir = Env.GetOrThrow("RESULTS_PATH");
        var filePath = Path.Combine(dir, $"{judge.Category.Slug}.xlsx");

        var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);

        return File(fs, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            Path.GetFileName(filePath));
    }
}