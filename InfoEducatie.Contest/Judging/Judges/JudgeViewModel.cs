using System.ComponentModel;
using InfoEducatie.Contest.Categories;
using InfoEducatie.Contest.Judging.JudgingCriteria;
using MCMS.Base.Attributes.JsonConverters;
using MCMS.Base.Data.ViewModels;
using MCMS.Base.Display.ModelDisplay;
using MCMS.Base.Display.ModelDisplay.Attributes;
using Newtonsoft.Json;

namespace InfoEducatie.Contest.Judging.Judges
{
    [DisplayName("Judge")]
    public class JudgeViewModel : ViewModel
    {
        [TableColumn(Invisible = true)] public override string Id { get; set; }

        [TableColumn(DbColumn = "User.FirstName", DbFuncFormat = "MDbFunctions.Concat({0}, ' ', x.User.LastName)")]
        public string FullName { get; set; }

        [DisplayName("Vice Pres.")]
        [TableColumn]
        public bool IsVicePresident { get; set; }

        public bool EmailConfirmed { get; set; }
        public string Email { get; set; }

        [TableColumn(DbColumn = "User.Email")]
        [DetailsField(Hidden = true)]
        public string EmailAddress => Email + " " + (EmailConfirmed ? "✔️" : "❌");

        [JsonConverter(typeof(ToStringJsonConverter))]
        [TableColumn(DbColumn = "Category.Name")]
        public CategoryViewModel Category { get; set; }

        [TableColumn] public JudgeType AvailableFor { get; set; }

        [TableColumn(Invisible = true, Searchable = ServerClient.None, Orderable = ServerClient.None)]
        public int PointsAddedProject { get; set; }
        [TableColumn(Invisible = true, Searchable = ServerClient.None, Orderable = ServerClient.None)]
        public int PointsAddedOpen { get; set; }

        public override string ToString()
        {
            return FullName;
        }
    }
}