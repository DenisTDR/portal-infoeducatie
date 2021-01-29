using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using InfoEducatie.Contest.Categories;
using MCMS.Base.Attributes.JsonConverters;
using MCMS.Base.Data.ViewModels;
using MCMS.Base.Display.ModelDisplay.Attributes;
using Newtonsoft.Json;

namespace InfoEducatie.Contest.Judging.JudgingCriteria.JudgingCriteriaSection
{
    [DisplayName("Criteria section")]
    public class JudgingCriteriaSectionViewModel : ViewModel
    {
        [JsonConverter(typeof(ToStringJsonConverter))]
        [TableColumn(DbColumn = "Category.Name")]
        public CategoryViewModel Category { get; set; }

        [TableColumn] public JudgingType Type { get; set; }
        [TableColumn] public string Name { get; set; }
        public string Description { get; set; }


        [DetailsField(Hidden = true)]
        [JsonIgnore]
        public List<JudgingCriterionViewModel> Criteria { get; set; }

        [JsonIgnore]
        [DetailsField(Hidden = true)]
        public int MaxPoints => Criteria?.Sum(c => c.MaxPoints) ?? 0;

        public override string ToString()
        {
            if (Category == null)
            {
                return Name;
            }

            return $"{Category.Name}: {Name}";
        }
    }
}