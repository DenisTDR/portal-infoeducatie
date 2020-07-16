using System.ComponentModel.DataAnnotations;
using InfoEducatie.Contest.Categories;
using MCMS.Base.Data.FormModels;
using MCMS.Base.SwaggerFormly.Formly.Fields;

namespace InfoEducatie.Contest.Participants.Project
{
    public class ProjectFormModel : IFormModel
    {
        [FormlySelect(typeof(CategoriesAdminApiController))]
        [Required]
        public CategoryViewModel Category { get; set; }

        [Required] public string Title { get; set; }
        [Required] [FormlyCkEditor] public string Description { get; set; }
        [Required] public string Technologies { get; set; }
        [Required] public string SystemRequirements { get; set; }
        public string SourceUrl { get; set; }
        public string Homepage { get; set; }
        public string OldPlatformId { get; set; }
    }
}