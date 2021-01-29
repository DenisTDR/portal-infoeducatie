using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using InfoEducatie.Contest.Categories;
using InfoEducatie.Contest.Participants.Participant;
using MCMS.Base.Attributes.JsonConverters;
using MCMS.Base.Data.ViewModels;
using MCMS.Base.Display.ModelDisplay;
using MCMS.Base.Display.ModelDisplay.Attributes;
using Newtonsoft.Json;

namespace InfoEducatie.Contest.Participants.Project
{
    [DisplayName("Project")]
    public class ProjectViewModel : ViewModel
    {
        [JsonConverter(typeof(ToStringJsonConverter))]
        [TableColumn(DbColumn = "Category.Name")]
        public CategoryViewModel Category { get; set; }

        [TableColumn] public string Title { get; set; }
        public string Description { get; set; }
        public string Technologies { get; set; }
        public string SystemRequirements { get; set; }

        [TableColumn(Orderable = ServerClient.None)]
        public string SourceUrl { get; set; }

        public string Homepage { get; set; }
        public string OldPlatformId { get; set; }
        public string DiscourseUrl { get; set; }
        [TableColumn] public bool IsInOpen { get; set; }
        public float ScoreProject { get; set; }
        public float ScoreOpen { get; set; }

        [DetailsField(Hidden = true)]
        [JsonIgnore]
        public List<ParticipantViewModel> Participants { get; set; }

        [TableColumn(DbColumn = "Participants",
            DbFuncFormat = "{0}.Any(p=> <condition>)<sel>MDbFunctions.Concat(p.User.FirstName, ' ', p.User.LastName)",
            Orderable = ServerClient.None)]
        [DisplayName("Participants")]
        public string ProjectsNames => Participants?.Count is { } nr && nr > 0
            ? string.Join(", ", Participants.Select(p => p.FullName))
            : "--";

        public override string ToString()
        {
            return Title;
        }
    }
}