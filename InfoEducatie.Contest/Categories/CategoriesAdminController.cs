using System.ComponentModel;
using MCMS.Controllers.Ui;
using Microsoft.AspNetCore.Authorization;

namespace InfoEducatie.Contest.Categories
{
    [Authorize(Roles = "Moderator")]
    [DisplayName("Categories")]
    public class CategoriesAdminController : GenericModalAdminUiController<CategoryEntity, CategoryFormModel,
        CategoryViewModel, CategoriesAdminApiController>
    {
    }
}