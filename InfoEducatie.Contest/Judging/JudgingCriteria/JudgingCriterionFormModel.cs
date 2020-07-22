using System.ComponentModel.DataAnnotations;
using InfoEducatie.Contest.Categories;
using InfoEducatie.Contest.Judging.JudgingCriteria.JudgingCriteriaSection;
using MCMS.Base.Data.FormModels;
using MCMS.Base.SwaggerFormly.Formly;
using MCMS.Base.SwaggerFormly.Formly.Fields;

namespace InfoEducatie.Contest.Judging.JudgingCriteria
{
    public class JudgingCriterionFormModel : IFormModel
    {
        [Required] public string Name { get; set; }

        [Required] [FormlyCkEditor] public string Description { get; set; }

        [Required] public int MaxPoints { get; set; }

        [FormlySelect(typeof(CategoriesAdminApiController), nameof(CategoriesAdminApiController.IndexLight))]
        [Required]
        public CategoryViewModel Category { get; set; }

        [FormlySelect(typeof(JudgingCriteriaSectionsAdminApiController),
            nameof(JudgingCriteriaSectionsAdminApiController.IndexLight))]
        [Required]
        public JudgingCriteriaSectionViewModel Section { get; set; }

        [FormlyFieldDefaultValue(JudgingType.Project)]
        public JudgingType Type { get; set; }
    }
}