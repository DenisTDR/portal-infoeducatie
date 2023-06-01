using System.ComponentModel;
using InfoEducatie.Contest.Categories;
using InfoEducatie.Contest.Judging.JudgingCriteria.JudgingCriteriaSection;
using MCMS.Base.Attributes.JsonConverters;
using MCMS.Base.Data.ViewModels;
using MCMS.Base.Display.ModelDisplay.Attributes;
using Newtonsoft.Json;

namespace InfoEducatie.Contest.Judging.JudgingCriteria
{
    [DisplayName("Judging Criterion")]
    public class JudgingCriterionViewModel : ViewModel
    {
        [TableColumn] public string Name { get; set; }
        public string Description { get; set; }
        [TableColumn] public int MaxPoints { get; set; }

        [DetailsField(Hidden = true)] public int Order { get; set; }

        [JsonConverter(typeof(ToStringJsonConverter))]
        [TableColumn(DbColumn = "Category.Name")]
        public CategoryViewModel Category { get; set; }

        [JsonConverter(typeof(ToStringJsonConverter))]
        [TableColumn(DbColumn = "Section.Name")]
        public JudgingCriteriaSectionViewModel Section { get; set; }

        [TableColumn] public JudgingType Type { get; set; }

        public override string ToString()
        {
            return (Section != null ? Section.Type + ": " + Section.Name + " -> " : "") +
                   Name + (Category != null ? $" ({Category})" : "");
        }
    }
}