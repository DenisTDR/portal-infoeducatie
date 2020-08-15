using System.ComponentModel.DataAnnotations;
using InfoEducatie.Contest.Categories;
using MCMS.Base.Data.FormModels;
using MCMS.Base.SwaggerFormly.Formly.Fields;

namespace InfoEducatie.Main.InfoEducatieAdmin
{
    public class CloneCriteriaFromProjectToOpenForCategoryFormModel : IFormModel
    {
        [FormlySelect(typeof(CategoriesAdminApiController), nameof(CategoriesAdminApiController.IndexLight))]
        [Required]
        public CategoryViewModel Category { get; set; }

        [FormlyField(DefaultValue = false)] public bool RemoveExistingOpenCriteriaFirst { get; set; }
    }
}