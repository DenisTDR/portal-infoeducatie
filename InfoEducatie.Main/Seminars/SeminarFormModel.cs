using System;
using System.ComponentModel.DataAnnotations;
using MCMS.Base.Data.FormModels;
using MCMS.Base.SwaggerFormly.Formly.Fields;

namespace InfoEducatie.Main.Seminars
{
    public class SeminarFormModel : IFormModel
    {
        [Required] public string Name { get; set; }
        [Required] public string Speaker { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime When { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string ShortDescription { get; set; }

        [Required] [FormlyCkEditor] public string Description { get; set; }
        [Required] public string Link { get; set; }

        [FormlyField(DefaultValue = true)] public bool Published { get; set; }
        [Required] public string Slug { get; set; }
    }
}