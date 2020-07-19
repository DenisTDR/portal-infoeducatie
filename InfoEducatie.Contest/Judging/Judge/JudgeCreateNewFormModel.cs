using System.ComponentModel.DataAnnotations;
using MCMS.Base.SwaggerFormly.Formly;

namespace InfoEducatie.Contest.Judging.Judge
{
    public class JudgeCreateNewFormModel : JudgeFormModel
    {
        [Required]
        [FormlyFieldProp("description", "Existing account email address.", true)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}