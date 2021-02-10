using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using InfoEducatie.Contest.Categories;
using MCMS.Base.Data.FormModels;
using MCMS.Base.SwaggerFormly.Formly;
using MCMS.Base.SwaggerFormly.Formly.Base;
using MCMS.Base.SwaggerFormly.Formly.Fields;

namespace InfoEducatie.Main.InfoEducatieAdmin
{
    public class CustomEmailToParticipantsModel : IFormModel
    {
        [FormlyField(DefaultValue = SendEmailToParticipantsType.Category)]
        public SendEmailToParticipantsType Type { get; set; }

        [FormlySelect(typeof(CategoriesAdminApiController))]
        [FormlyFieldProp("hideExpression", "model.type !== 'category'")]
        public CategoryViewModel Category { get; set; }

        [Required] public string Subject { get; set; }
        [Required] [FormlyCkEditor] public string Message { get; set; }
        [FormlyField(DefaultValue = false)] public bool SendIt { get; set; }
    }

    public enum SendEmailToParticipantsType
    {
        [EnumMember(Value = "category")] Category,
        [EnumMember(Value = "all")] All
    }
}