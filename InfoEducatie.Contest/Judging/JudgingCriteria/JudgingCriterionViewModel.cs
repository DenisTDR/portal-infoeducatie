using System.ComponentModel;
using InfoEducatie.Contest.Categories;
using InfoEducatie.Contest.Judging.JudgingCriteria.JudgingCriteriaSection;
using MCMS.Base.Attributes;
using MCMS.Base.Data.ViewModels;
using MCMS.Base.Display.ModelDisplay.Attributes;
using Newtonsoft.Json;

namespace InfoEducatie.Contest.Judging.JudgingCriteria
{
    [DisplayName("Judging Criterion")]
    public class JudgingCriterionViewModel : ViewModel
    {
        public string Name { get; set; }
        [TableColumn(Hidden = true)] public string Description { get; set; }
        public int MaxPoints { get; set; }

        [DetailsField(Hidden = true)]
        [TableColumn(Hidden = true)]
        public int Order { get; set; }

        [JsonConverter(typeof(ToStringJsonConverter))]
        public CategoryViewModel Category { get; set; }

        [JsonConverter(typeof(ToStringJsonConverter))]
        public JudgingCriteriaSectionViewModel Section { get; set; }

        public JudgingType Type { get; set; }

        public override string ToString()
        {
            return Name + (Category != null ? $" ({Category})" : "");
        }
    }
}