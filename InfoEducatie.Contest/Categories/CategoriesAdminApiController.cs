using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MCMS.Controllers.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InfoEducatie.Contest.Categories
{
    [Authorize(Roles = "Moderator")]
    public class
        CategoriesAdminApiController : GenericAdminApiController<CategoryEntity, CategoryFormModel, CategoryViewModel>
    {
        public override Task<ActionResult<List<CategoryViewModel>>> IndexLight()
        {
            Repo.ChainQueryable(q => q.Select(e => new CategoryEntity {Id = e.Id, Name = e.Name}));
            return base.Index();
        }
    }
}