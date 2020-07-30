using System.ComponentModel.DataAnnotations;
using InfoEducatie.Contest.Categories;
using MCMS.Base.Data.FormModels;
using MCMS.Base.SwaggerFormly.Formly;
using MCMS.Base.SwaggerFormly.Formly.Fields;

namespace InfoEducatie.Contest.Participants.Project
{
    public class ProjectFormModel : IFormModel
    {
        [FormlySelect(typeof(CategoriesAdminApiController))]
        // [Required]
        public CategoryViewModel Category { get; set; }

        [Required] public string Title { get; set; }
        [Required] [FormlyCkEditor] public string Description { get; set; }
        [Required] [FormlyCkEditor] public string Technologies { get; set; }
        [Required] [FormlyCkEditor] public string SystemRequirements { get; set; }
        public string SourceUrl { get; set; }
        public string Homepage { get; set; }
        public string OldPlatformId { get; set; }
        public string DiscourseUrl { get; set; }
        [FormlyFieldDefaultValue(false)] public bool IsInOpen { get; set; }
        
        public float ScoreProject { get; set; }
        public float ScoreOpen { get; set; }
    }
}