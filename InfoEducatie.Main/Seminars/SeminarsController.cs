using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MCMS.Base.Data;
using MCMS.Base.Extensions;
using MCMS.Controllers.Ui;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace InfoEducatie.Main.Seminars
{
    [Route("/[controller]")]
    public class SeminarsController : UiController
    {
        protected IRepository<SeminarEntity> Repo => ServiceProvider.GetRepo<SeminarEntity>();

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            Repo.ChainQueryable(q => q.Where(s => s.Published));
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var all = await Repo.GetAll();
            var vms = Mapper.Map<List<SeminarViewModel>>(all);
            return View(vms);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Details([FromRoute] string id)
        {
            var seminar = await Repo.GetOneOrThrow(id);
            var vm = Mapper.Map<SeminarViewModel>(seminar);
            return View(vm);
        }
    }
}