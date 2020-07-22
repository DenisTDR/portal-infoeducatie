using System.ComponentModel;
using System.Threading.Tasks;
using MCMS.Controllers.Ui;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InfoEducatie.Contest.Judging.JudgingCriteria.JudgingCriteriaSection
{
    [DisplayName("Crieria sections")]
    public class JudgingCriteriaSectionsAdminController : GenericModalAdminUiController<JudgingCriteriaSectionEntity,
        JudgingCriteriaSectionFormModel, JudgingCriteriaSectionViewModel, JudgingCriteriaSectionsAdminApiController>
    {
        public override Task<IActionResult> Details(string id)
        {
            Repo.ChainQueryable(q => q.Include(p => p.Category));
            return base.Details(id);
        }
    }
}