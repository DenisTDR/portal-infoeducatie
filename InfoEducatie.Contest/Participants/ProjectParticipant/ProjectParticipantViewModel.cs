using System.ComponentModel;
using InfoEducatie.Contest.Participants.Participant;
using InfoEducatie.Contest.Participants.Project;
using MCMS.Base.Attributes.JsonConverters;
using MCMS.Base.Data.ViewModels;
using Newtonsoft.Json;

namespace InfoEducatie.Contest.Participants.ProjectParticipant
{
    [DisplayName("Project Participant")]
    public class ProjectParticipantViewModel : ViewModel
    {
        [JsonConverter(typeof(ToStringJsonConverter))]
        public ProjectViewModel Project { get; set; }

        [JsonConverter(typeof(ToStringJsonConverter))]
        public ParticipantViewModel Participant { get; set; }

        public override string ToString()
        {
            return Project + "->" + Participant;
        }
    }
}