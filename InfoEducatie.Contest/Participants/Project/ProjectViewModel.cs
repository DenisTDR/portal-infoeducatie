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
        [TableColumn(Invisible = true)]
        [DetailsField]
        public override string Id { get; set; }

        [JsonConverter(typeof(ToStringJsonConverter))]
        [TableColumn(DbColumn = "Category.Name")]
        [DetailsField]

        public CategoryViewModel Category { get; set; }

        [TableColumn] [DetailsField] public string Title { get; set; }
        [DetailsField] public string Description { get; set; }
        [DetailsField] public string Technologies { get; set; }
        [DetailsField] public string SystemRequirements { get; set; }

        [TableColumn(Orderable = ServerClient.None)]
        [DetailsField]

        public string SourceUrl { get; set; }

        [DetailsField] public string Homepage { get; set; }
        [DetailsField] public string OldPlatformId { get; set; }
        [DetailsField] public string DiscourseUrl { get; set; }
        [DetailsField] [TableColumn] public bool IsInOpen { get; set; }

        [TableColumn(Invisible = true)]
        [DetailsField]
        public float ScoreProject { get; set; }

        [TableColumn(Invisible = true)]
        [DetailsField]
        public float ScoreOpen { get; set; }

        [TableColumn(Invisible = true)]
        [DetailsField]
        public float TotalScore => ScoreProject + ScoreOpen;

        [TableColumn(Invisible = true)]
        [DetailsField]
        public string FinalPrize { get; set; }

        [DetailsField(Hidden = true)]
        [JsonIgnore]
        public List<ParticipantViewModel> Participants { get; set; }

        [TableColumn(DbColumn = "Participants",
            DbFuncFormat = "{0}.Any(p=> <condition>)<sel>MDbFunctions.Concat(p.User.FirstName, ' ', p.User.LastName)",
            Orderable = ServerClient.None)]
        [DisplayName("Participants")]
        [DetailsField]

        public string ProjectsNames => Participants?.Count is { } nr && nr > 0
            ? string.Join(", ", Participants.Select(p => p.FullName))
            : "--";

        public override string ToString()
        {
            return Title;
        }
    }
}