using System.ComponentModel.DataAnnotations;
using InfoEducatie.Contest.Categories;
using MCMS.Base.Data.FormModels;
using MCMS.Base.SwaggerFormly.Formly.Fields;

namespace InfoEducatie.Contest.Judging.JudgingCriteria
{
    public class JudgingCriterionFormModel : IFormModel
    {
        [Required] public string Name { get; set; }

        [Required] [FormlyCkEditor] public string Description { get; set; }

        [Required] public int MaxPoints { get; set; }

        [FormlySelect(typeof(CategoriesAdminApiController))]
        [Required]
        public CategoryViewModel Category { get; set; }
    }
}