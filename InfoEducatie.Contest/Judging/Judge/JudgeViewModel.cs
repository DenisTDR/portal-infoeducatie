using System.ComponentModel;
using InfoEducatie.Contest.Categories;
using MCMS.Base.Attributes;
using MCMS.Base.Data.ViewModels;
using Newtonsoft.Json;

namespace InfoEducatie.Contest.Judging.Judge
{
    [DisplayName("Judge")]
    public class JudgeViewModel : ViewModel
    {
        [JsonConverter(typeof(ToStringJsonConverter))]
        public CategoryViewModel Category { get; set; }

        public string FullName { get; set; }

        public override string ToString()
        {
            return FullName;
        }
    }
}