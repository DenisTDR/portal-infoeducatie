using System.ComponentModel.DataAnnotations;
using InfoEducatie.Contest.Categories;
using MCMS.Base.Data.FormModels;
using MCMS.Base.SwaggerFormly.Formly.Fields;

namespace InfoEducatie.Contest.Judging.Judge
{
    public class JudgeFormModel : IFormModel
    {
        [Required]
        [FormlySelect(typeof(CategoriesAdminApiController))]
        public CategoryViewModel Category { get; set; }

    }
}