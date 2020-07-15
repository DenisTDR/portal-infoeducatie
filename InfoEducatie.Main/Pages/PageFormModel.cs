using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MCMS.Base.Data.FormModels;
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

        [Required] [FormlyCkEditor] public string Body { get; set; }
        public bool Published { get; set; }
    }
}