using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfoEducatie.Contest.Categories;
using InfoEducatie.Contest.Judging.JudgingCriteria.JudgingCriteriaSection;
using MCMS.Base.Exceptions;
using MCMS.Base.Extensions;
using MCMS.Controllers.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace InfoEducatie.Contest.Judging.JudgingCriteria
{
    [Authorize(Roles = "Moderator")]
    public class JudgingCriteriaAdminApiController : GenericAdminApiController<JudgingCriterionEntity,
        JudgingCriterionFormModel, JudgingCriterionViewModel>
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            Repo.ChainQueryable(q => q
                .Include(jc => jc.Category)
                .Include(jc => jc.Section)
                .OrderBy(jc => jc.Category.Name).ThenBy(jc => jc.Section.Type).ThenBy(jc => jc.Section.Name)
                .ThenBy(jc => jc.Name)
            );
        }

        protected override Task OnCreating(JudgingCriterionEntity e)
        {
            if (e.Category == null)
            {
                throw new KnownException("Invalid category.");
            }

            if (e.Section == null)
            {
                throw new KnownException("Invalid section.");
            }

            ServiceProvider.GetRepo<CategoryEntity>().Attach(e.Category);
            ServiceProvider.GetRepo<JudgingCriteriaSectionEntity>().Attach(e.Section);
            return Task.CompletedTask;
        }

        public override Task<ActionResult<List<JudgingCriterionViewModel>>> IndexLight()
        {
            Repo.ChainQueryable(q => q.Select(e => new JudgingCriterionEntity {Id = e.Id, Name = e.Name}));
            return base.Index();
        }
    }
}