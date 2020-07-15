using System.ComponentModel.DataAnnotations;
using MCMS.Base.Data.FormModels;
using MCMS.Base.SwaggerFormly.Formly.Fields;

namespace InfoEducatie.Main.Seminars
{
    public class SeminarFormModel : IFormModel
    {
        public string Name { get; set; }
        public string Speaker { get; set; }
        [DataType(DataType.MultilineText)] public string ShortDescription { get; set; }
        [FormlyCkEditor] public string Description { get; set; }
        public string Link { get; set; }
    }
}