using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using InfoEducatie.Contest.Categories;
using MCMS.Base.Attributes;
using MCMS.Base.Data.ViewModels;
using Newtonsoft.Json;

namespace InfoEducatie.Contest.Judging.JudgingCriteria.JudgingCriteriaSection
{
    [DisplayName("Criteria section")]
    public class JudgingCriteriaSectionViewModel : ViewModel
    {
        [JsonConverter(typeof(ToStringJsonConverter))]
        public CategoryViewModel Category { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            if (Category == null)
            {
                return Name;
            }

            return Category.Name + ": " + Name;
        }
    }
}