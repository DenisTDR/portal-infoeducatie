using System.ComponentModel.DataAnnotations;
using InfoEducatie.Contest.Categories;
using InfoEducatie.Contest.Judging.JudgingCriteria;
using MCMS.Base.Data.FormModels;
using MCMS.Base.SwaggerFormly.Formly.Fields;

namespace InfoEducatie.Contest.Judging.Judges
{
    public class JudgeFormModel : IFormModel
    {
        [Required]
        [FormlySelect(typeof(CategoriesAdminApiController))]
        public CategoryViewModel Category { get; set; }

        [FormlyField(DefaultValue = false)] public bool IsVicePresident { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }


        [FormlyField(DefaultValue = JudgeType.Both)]
        public JudgeType AvailableFor { get; set; }
    }
}