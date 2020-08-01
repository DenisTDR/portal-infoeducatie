using System.ComponentModel.DataAnnotations;
using MCMS.Base.Data.FormModels;
using MCMS.Base.SwaggerFormly.Formly;

namespace InfoEducatie.Contest.Categories
{
    public class CategoryFormModel : IFormModel
    {
        [Required] public string Name { get; set; }

        [Required] public string Slug { get; set; }
        [FormlyFieldDefaultValue(true)] public bool Published { get; set; }
        public bool ScoresX10 { get; set; }
    }
}