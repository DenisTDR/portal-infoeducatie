using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using InfoEducatie.Contest.Judging.Judge;
using InfoEducatie.Contest.Judging.JudgingCriteria;
using InfoEducatie.Contest.Participants.Project;
using MCMS.Base.Attributes;
using MCMS.Base.Data.ViewModels;
using Newtonsoft.Json;

namespace InfoEducatie.Contest.Judging.ProjectJudgingCriterionPoints
{
    [DisplayName("Project Judging Criterion Points")]
    public class ProjectJudgingCriterionPointsViewModel : ViewModel
    {
        [JsonConverter(typeof(ToStringJsonConverter))]
        public JudgeViewModel Judge { get; set; }

        [JsonConverter(typeof(ToStringJsonConverter))]
        public JudgingCriterionViewModel Criterion { get; set; }

        [JsonConverter(typeof(ToStringJsonConverter))]
        public ProjectViewModel Project { get; set; }

        [Required] public int Points { get; set; }

        public override string ToString()
        {
            return $"{Points} for {Project?.Title} from {Judge?.FullName} in {Criterion?.Name}";
        }
    }
}