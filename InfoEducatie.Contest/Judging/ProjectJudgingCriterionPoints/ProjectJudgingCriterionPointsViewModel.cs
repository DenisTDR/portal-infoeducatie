using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using InfoEducatie.Contest.Categories;
using InfoEducatie.Contest.Judging.Judges;
using InfoEducatie.Contest.Judging.JudgingCriteria;
using InfoEducatie.Contest.Participants.Project;
using MCMS.Base.Attributes.JsonConverters;
using MCMS.Base.Data.ViewModels;
using MCMS.Base.Display.ModelDisplay.Attributes;
using Newtonsoft.Json;

namespace InfoEducatie.Contest.Judging.ProjectJudgingCriterionPoints
{
    [DisplayName("Project Judging Criterion Points")]
    public class ProjectJudgingCriterionPointsViewModel : ViewModel
    {
        [JsonConverter(typeof(ToStringJsonConverter))]
        [TableColumn(DbColumn = "Judge.User.FirstName",
            DbFuncFormat = "MDbFunctions.Concat({0}, ' ', x.Judge.User.LastName)")]
        public JudgeViewModel Judge { get; set; }

        [JsonConverter(typeof(ToStringJsonConverter))]
        [TableColumn(DbColumn = "Criterion.Category.Name")]
        public CategoryViewModel Category { get; set; }

        [JsonConverter(typeof(ToStringJsonConverter))]
        [TableColumn(DbColumn = "Criterion.Name",
            DbFuncFormat = "MDbFunctions.Concat({0}, ' (', x.Criterion.Category.Name, ')')")]
        public JudgingCriterionViewModel Criterion { get; set; }

        [JsonConverter(typeof(ToStringJsonConverter))]
        [TableColumn(DbColumn = "Project.Title")]
        public ProjectViewModel Project { get; set; }

        [TableColumn] [Required] public int Points { get; set; }

        public override string ToString()
        {
            return $"{Points} for {Project?.Title} from {Judge?.FullName} in {Criterion?.Name}";
        }
    }
}