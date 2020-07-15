using System.Threading.Tasks;
using InfoEducatie.Contest.Categories;
using MCMS.Base.Exceptions;
using MCMS.Controllers.Api;
using MCMS.Data;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InfoEducatie.Contest.Judging.JudgingCriteria
{
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
    }
}