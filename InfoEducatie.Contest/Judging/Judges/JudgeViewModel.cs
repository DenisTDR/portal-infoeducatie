using System.ComponentModel;
using InfoEducatie.Contest.Categories;
using InfoEducatie.Contest.Judging.JudgingCriteria;
using MCMS.Base.Attributes;
using MCMS.Base.Data.ViewModels;
using MCMS.Base.Display.ModelDisplay.Attributes;
using Newtonsoft.Json;

namespace InfoEducatie.Contest.Judging.Judges
{
    [DisplayName("Judge")]
    public class JudgeViewModel : ViewModel
    {
        public string FullName { get; set; }
        [TableColumn(Hidden = true)] public bool EmailConfirmed { get; set; }
        [TableColumn(Hidden = true)] public string Email { get; set; }

        [DetailsField(Hidden = true)] public string EmailAddress => Email + (EmailConfirmed ? "✔️" : "❌");

        [JsonConverter(typeof(ToStringJsonConverter))]
        public CategoryViewModel Category { get; set; }


        public JudgeType AvailableFor { get; set; }

        public override string ToString()
        {
            return FullName;
        }
    }
}