using System.ComponentModel.DataAnnotations;
using InfoEducatie.Contest.Categories;
using InfoEducatie.Contest.Judging.JudgingCriteria;
using MCMS.Base.Data.FormModels;
using MCMS.Base.SwaggerFormly.Formly;
using MCMS.Base.SwaggerFormly.Formly.Fields;

namespace InfoEducatie.Contest.Judging.Judge
{
    public class JudgeFormModel : IFormModel
    {
        [Required]
        [FormlySelect(typeof(CategoriesAdminApiController))]
        public CategoryViewModel Category { get; set; }

        [FormlyFieldDefaultValue(JudgeType.Both)]
        public JudgeType AvailableFor { get; set; }
    }
}