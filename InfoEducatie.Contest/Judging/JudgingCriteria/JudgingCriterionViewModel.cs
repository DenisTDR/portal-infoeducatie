using System.ComponentModel;
using InfoEducatie.Contest.Categories;
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
        public string Description { get; set; }
        public int MaxPoints { get; set; }
        public int Order { get; set; }

        [JsonConverter(typeof(ToStringJsonConverter))]
        public CategoryViewModel Category { get; set; }

        public override string ToString()
        {
            return Name + (Category != null ? $" ({Category})" : "");
        }
    }
}