using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfoEducatie.Contest.Categories;
using MCMS.Base.Auth;
using MCMS.Controllers.Api;
using MCMS.Data;
using Microsoft.AspNetCore.Identity;
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

        public override async Task<ActionResult<JudgeFormModel>> Create(JudgeFormModel fm)
        {
            var user = await ServiceProvider.GetService<IRepository<User>>()
                .GetOne(u => u.NormalizedEmail == fm.Email.ToUpper());
            if (user == null)
            {
                return BadRequest($"There is no user with email '{fm.Email}'");
            }

            if (await Repo.Any(j => j.User == user))
            {
                return BadRequest($"This user is already a judge.");
            }

            var userManager = ServiceProvider.GetService<UserManager<User>>();
            if (!await userManager.IsInRoleAsync(user, "Jury"))
            {
                await userManager.AddToRoleAsync(user, "Jury");
            }

            var e = new JudgeEntity
            {
                Category = ServiceProvider.GetService<IRepository<CategoryEntity>>()
                    .Attach(new CategoryEntity {Id = fm.Category.Id}),
                User = user
            };
            e = await Repo.Add(e);
            var vm = MapF(e);
            return Ok(vm);
        }
    }
}