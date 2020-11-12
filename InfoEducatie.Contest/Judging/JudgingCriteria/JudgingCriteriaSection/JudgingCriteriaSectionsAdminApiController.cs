using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfoEducatie.Contest.Categories;
using MCMS.Base.Extensions;
using MCMS.Controllers.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

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

        protected override Task OnCreating(JudgingCriteriaSectionEntity e)
        {
            ServiceProvider.GetRepo<CategoryEntity>().Attach(e.Category);
            return base.OnCreating(e);
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