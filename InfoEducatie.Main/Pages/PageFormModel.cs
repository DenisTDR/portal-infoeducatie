using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MCMS.Base.Data.FormModels;
using MCMS.Base.SwaggerFormly.Formly.Base;
using MCMS.Base.SwaggerFormly.Formly.Fields;

namespace InfoEducatie.Main.Pages
{
    public class PageFormModel : IFormModel
    {
        [Required] public string Slug { get; set; }

        [Required] public string Title { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string ShortDescription { get; set; }

        [FormlyField(DefaultValue = false)] public bool IsPlainHtml { get; set; }


        [FormlyFieldProp("hideExpression", "model.isPlainHtml")]
        [FormlyCkEditor]
        [Required]
        public string Body { get; set; }

        [FormlyFieldProp("hideExpression", "!model.isPlainHtml")]
        [DataType(DataType.MultilineText)]
        [FormlyFieldProp("clone-key", "body")]
        [DisplayName("Body")]
        public string BodyHtml { get; set; }

        [FormlyField(DefaultValue = true)] public bool Published { get; set; }

        [FormlyField(DefaultValue = true)] public bool DisplayTitle { get; set; }
    }
}