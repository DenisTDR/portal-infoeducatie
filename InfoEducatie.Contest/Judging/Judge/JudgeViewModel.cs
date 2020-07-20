using System.ComponentModel;
using InfoEducatie.Contest.Categories;
using InfoEducatie.Contest.Judging.JudgingCriteria;
using MCMS.Base.Attributes;
using MCMS.Base.Data.ViewModels;
using Newtonsoft.Json;

namespace InfoEducatie.Contest.Judging.Judge
{
    [DisplayName("Judge")]
    public class JudgeViewModel : ViewModel
    {
        public string FullName { get; set; }

        [JsonConverter(typeof(ToStringJsonConverter))]
        public CategoryViewModel Category { get; set; }


        public JudgingType AvailableFor { get; set; }

        public override string ToString()
        {
            return FullName;
        }
    }
}