using System.ComponentModel.DataAnnotations.Schema;
using InfoEducatie.Contest.Categories;
using MCMS.Base.Data.Entities;

namespace InfoEducatie.Contest.Participants.Project
{
    [Table("Projects")]
    public class ProjectEntity : Entity
    {
        public CategoryEntity Category { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Technologies { get; set; }
        public string SystemRequirements { get; set; }
        public string SourceUrl { get; set; }
        public string Homepage { get; set; }
        public string OldPlatformId { get; set; }
        public bool IsInOpen { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }
}