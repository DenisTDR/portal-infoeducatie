using System.ComponentModel.DataAnnotations;
using InfoEducatie.Contest.Categories;
using MCMS.Base.Data.FormModels;
using MCMS.Base.SwaggerFormly.Formly.Fields;

namespace InfoEducatie.Contest.Judging.JudgingCriteria.JudgingCriteriaSection
{
    public class JudgingCriteriaSectionFormModel : IFormModel
    {
        [FormlySelect(typeof(CategoriesAdminApiController), nameof(CategoriesAdminApiController.IndexLight))]
        [Required]
        public CategoryViewModel Category { get; set; }

        public string Name { get; set; }
        [FormlyCkEditor] public string Description { get; set; }
        public JudgingType Type { get; set; }
    }
}