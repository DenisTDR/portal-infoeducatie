using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfoEducatie.Contest.Categories;
using InfoEducatie.Contest.Judging.JudgingCriteria;
using InfoEducatie.Contest.Judging.ProjectJudgingCriterionPoints;
using MCMS.Auth;
using MCMS.Base.Attributes;
using MCMS.Base.Auth;
using MCMS.Base.Extensions;
using MCMS.Controllers.Api;
using MCMS.Models;
using MCMS.Models.Dt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InfoEducatie.Contest.Judging.Judges
{
    [Authorize(Roles = "Moderator")]
    public class JudgesAdminApiController : CrudAdminApiController<JudgeEntity, JudgeFormModel, JudgeViewModel>
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            Repo.ChainQueryable(q => q
                .Include(j => j.Category)
                .Include(j => j.User)
            );
        }

        protected override Task OnCreating(JudgeEntity e)
        {
            ServiceProvider.GetRepo<CategoryEntity>().Attach(e.Category);
            return base.OnCreating(e);
        }

        public override async Task<ActionResult<List<JudgeViewModel>>> Index()
        {
            var all = await Repo.GetAll();
            var allVm = Map(all).OrderBy(j => j.FullName);
            return Ok(allVm);
        }

        public override async Task<ActionResult<List<JudgeViewModel>>> IndexLight()
        {
            var all = await Repo.GetAll();
            var allVm = all.Select(e => new JudgeViewModel { FullName = e.FullName, Id = e.Id })
                .OrderBy(j => j.FullName);
            return Ok(allVm);
        }

        [NonAction]
        public override Task<ActionResult<ModelResponse<JudgeFormModel>>> Create(JudgeFormModel fm)
        {
            throw new NotImplementedException();
        }


        [HttpPost]
        [ModelValidation]
        public async Task<ActionResult<ModelResponse<JudgeFormModel>>> Create([FromBody] JudgeCreateNewFormModel fm)
        {
            var userManager = ServiceProvider.GetRequiredService<UserManager<User>>();
            var user = await ServiceProvider.GetRepo<User>()
                .GetOne(u => u.NormalizedEmail == fm.Email.ToUpper());
            if (user == null)
            {
                user = new User { UserName = fm.Email, Email = fm.Email };
                var result = await userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }

                await ServiceProvider.GetRequiredService<AuthService>().SendActivationEmail(user, Url, Request.Scheme);
            }

            user.LastName = fm.LastName;
            user.FirstName = fm.FirstName;

            if (await Repo.Any(j => j.User == user))
            {
                return BadRequest($"This user is already a judge.");
            }

            if (!await userManager.IsInRoleAsync(user, "Jury"))
            {
                await userManager.AddToRoleAsync(user, "Jury");
            }

            var e = new JudgeEntity
            {
                Category = ServiceProvider.GetRepo<CategoryEntity>()
                    .Attach(new CategoryEntity { Id = fm.Category.Id }),
                AvailableFor = fm.AvailableFor,
                User = user
            };
            e = await Repo.Add(e);
            var vm = MapF(e);
            return OkModel(vm);
        }

        public override async Task<ActionResult<DtResult<JudgeViewModel>>> DtQuery(DtParameters model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await QueryService.Query(Repo, model);


            var judgesIds = result.Data.Select(j => j.Id).ToList();
            var pointsRepo = Repo<ProjectJudgingCriterionPointsEntity>();
            var pointsList = await pointsRepo.Query
                .Where(p => judgesIds.Contains(p.Judge.Id))
                .GroupBy(p => p.Judge.Id)
                .Select(g => new
                {
                    judgeId = g.Key,
                    pointsProject = g.Count(x => x.Criterion.Type == JudgingType.Project),
                    pointsOpen = g.Count(x => x.Criterion.Type == JudgingType.Open),
                })
                .ToListAsync();
            foreach (var judge in result.Data)
            {
                judge.PointsAddedProject = pointsList.FirstOrDefault(p => p.judgeId == judge.Id)?.pointsProject ?? 0;
                judge.PointsAddedOpen = pointsList.FirstOrDefault(p => p.judgeId == judge.Id)?.pointsOpen ?? 0;
            }

            return result;
        }
    }
}