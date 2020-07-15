using MCMS.Controllers.Api;
using Microsoft.AspNetCore.Authorization;

namespace InfoEducatie.Contest.Categories
{
    [Authorize(Roles = "Moderator")]
    public class
        CategoriesAdminApiController : GenericAdminApiController<CategoryEntity, CategoryFormModel, CategoryViewModel>
    {
    }
}