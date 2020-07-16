using System.ComponentModel;
using InfoEducatie.Contest.Categories;
using MCMS.Base.Attributes;
using MCMS.Base.Data.ViewModels;
using MCMS.Base.Display.ModelDisplay.Attributes;
using Newtonsoft.Json;

namespace InfoEducatie.Contest.Participants.Project
{
    [DisplayName("Project")]
    public class ProjectViewModel : ViewModel
    {
        [JsonConverter(typeof(ToStringJsonConverter))]
        [TableColumn]
        public CategoryViewModel Category { get; set; }

        [TableColumn] public string Title { get; set; }
        public string Description { get; set; }
        public string Technologies { get; set; }
        public string SystemRequirements { get; set; }

        [TableColumn] public string SourceUrl { get; set; }
        public string Homepage { get; set; }
        public string OldPlatformId { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }
}