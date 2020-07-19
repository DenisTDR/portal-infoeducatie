using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfoEducatie.Contest.Categories;
using MCMS.Base.Exceptions;
using MCMS.Controllers.Api;
using MCMS.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InfoEducatie.Contest.Judging.JudgingCriteria
{
    [Authorize(Roles = "Moderator")]
    public class JudgingCriteriaAdminApiController : GenericAdminApiController<JudgingCriterionEntity,
        JudgingCriterionFormModel, JudgingCriterionViewModel>
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            Repo.ChainQueryable(q => q.Include(jc => jc.Category));
        }

        protected override Task PatchBeforeSaveNew(JudgingCriterionEntity e)
        {
            if (e.Category == null)
            {
                throw new KnownException("Invalid category.", 400);
            }

            ServiceProvider.GetService<IRepository<CategoryEntity>>().Attach(e.Category);
            return Task.CompletedTask;
        }

        public override Task<ActionResult<List<JudgingCriterionViewModel>>> IndexLight()
        {
            Repo.ChainQueryable(q => q.Select(e => new JudgingCriterionEntity {Id = e.Id, Name = e.Name}));
            return base.Index();
        }
    }
}