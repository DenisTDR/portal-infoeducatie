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

namespace InfoEducatie.Contest.Judging.JudgingCriteria.JudgingCriteriaSection
{
    public class JudgingCriteriaSectionsAdminApiController : GenericAdminApiController<JudgingCriteriaSectionEntity,
        JudgingCriteriaSectionFormModel, JudgingCriteriaSectionViewModel>
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            Repo.ChainQueryable(q => q
                .Include(p => p.Category)
                .OrderBy(jc => jc.Category.Name)
                .ThenBy(jc => jc.Type).ThenBy(jc => jc.Name)
            );
        }

        protected override Task PatchBeforeSaveNew(JudgingCriteriaSectionEntity e)
        {
            ServiceProvider.GetService<IRepository<CategoryEntity>>().Attach(e.Category);
            return base.PatchBeforeSaveNew(e);
        }

        public override async Task<ActionResult<List<JudgingCriteriaSectionViewModel>>> IndexLight()
        {
            var all = await Repo.GetAll();
            var allVm = all.Select(e => new JudgingCriteriaSectionViewModel
                {Name = e.ToString(), Id = e.Id}).OrderBy(j => j.Name);
            return Ok(allVm);
        }
    }
}