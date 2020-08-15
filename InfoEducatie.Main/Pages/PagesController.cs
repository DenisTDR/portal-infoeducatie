using System.Linq;
using System.Threading.Tasks;
using MCMS.Base.Data;
using MCMS.Base.Extensions;
using MCMS.Controllers.Ui;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace InfoEducatie.Main.Pages
{
    [Route("/[controller]")]
    public class PagesController : UiController
    {
        protected IRepository<PageEntity> Repo => ServiceProvider.GetRepo<PageEntity>();

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            Repo.ChainQueryable(q => q.Where(s => s.Published));
        }
        
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Details([FromRoute] string id)
        {
            var seminar = await Repo.GetOneOrThrow(id);
            var vm = Mapper.Map<PageViewModel>(seminar);
            return View(vm);
        }
    }
}