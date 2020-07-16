using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfoEducatie.Contest.Categories;
using MCMS.Controllers.Api;
using MCMS.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InfoEducatie.Contest.Judging.Judge
{
    public class JudgesAdminApiController : GenericAdminApiController<JudgeEntity, JudgeFormModel, JudgeViewModel>
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            Repo.ChainQueryable(q => q
                .Include(j => j.Category)
                .Include(j => j.User)
            );
        }

        protected override Task PatchBeforeSaveNew(JudgeEntity e)
        {
            ServiceProvider.GetService<IRepository<CategoryEntity>>().Attach(e.Category);
            return base.PatchBeforeSaveNew(e);
        }

        public override async Task<ActionResult<List<JudgeViewModel>>> IndexLight()
        {
            var all = await Repo.GetAll();
            var allVm = all.Select(e => new JudgeViewModel {FullName = e.FullName, Id = e.Id});
            return Ok(allVm);
        }
    }
}