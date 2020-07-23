using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using InfoEducatie.Contest.Categories;
using MCMS.Base.Attributes;
using MCMS.Base.Data.ViewModels;
using MCMS.Base.Display.ModelDisplay.Attributes;
using Newtonsoft.Json;

namespace InfoEducatie.Contest.Judging.JudgingCriteria.JudgingCriteriaSection
{
    [DisplayName("Criteria section")]
    public class JudgingCriteriaSectionViewModel : ViewModel
    {
        [JsonConverter(typeof(ToStringJsonConverter))]
        public CategoryViewModel Category { get; set; }

        public JudgingType Type { get; set; }

        public string Name { get; set; }
        [TableColumn(Hidden = true)] public string Description { get; set; }

        [TableColumn(Hidden = true)]
        [DetailsField(Hidden = true)]
        [JsonIgnore]
        public List<JudgingCriterionViewModel> Criteria { get; set; }

        [JsonIgnore]
        [TableColumn(Hidden = true)]
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