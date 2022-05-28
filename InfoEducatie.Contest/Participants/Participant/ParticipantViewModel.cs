using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using InfoEducatie.Contest.Participants.Project;
using MCMS.Base.Data.ViewModels;
using MCMS.Base.Display.ModelDisplay;
using MCMS.Base.Display.ModelDisplay.Attributes;
using Newtonsoft.Json;

namespace InfoEducatie.Contest.Participants.Participant
{
    [DisplayName("Participant")]
    public class ParticipantViewModel : ViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [TableColumn(DbColumn = "User.FirstName", DbFuncFormat = "MDbFunctions.Concat({0}, ' ', x.User.LastName)")]
        public string FullName => FirstName != null ? FirstName + " " + LastName : PhoneNumber;

        [TableColumn(Invisible = true)] public string PhoneNumber { get; set; }
        [TableColumn] public int Grade { get; set; }
        [TableColumn] public string City { get; set; }
        [TableColumn] public string County { get; set; }
        public string Country { get; set; }
        [TableColumn(Invisible = true)] public string School { get; set; }
        [TableColumn(Invisible = true)] public string SchoolCity { get; set; }
        [TableColumn(Invisible = true)] public string SchoolCounty { get; set; }
        [TableColumn(Invisible = true)] public string SchoolCountry { get; set; }
        [TableColumn(Invisible = true)] public string MentoringTeacher { get; set; }
        public string Cnp { get; set; }
        public string IdCardSeries { get; set; }
        public string IdCardNumber { get; set; }
        public string OldPlatformId { get; set; }
        public bool ActivationEmailSent { get; set; }

        public SentMailsState SentMails { get; set; }

        [TableColumn(Invisible = true, DbColumn = "User.Email")]
        public string Email { get; set; }

        [DetailsField(Hidden = true)]
        [JsonIgnore]
        public List<ProjectViewModel> Projects { get; set; }

        [TableColumn(DbColumn = "Projects",
            DbFuncFormat = "{0}.Any(p=> <condition>)<sel>p.Title", Orderable = ServerClient.None)]
        [DisplayName("Projects")]
        public string ProjectsNames => Projects?.Count is { } nr && nr > 0
            ? string.Join(", ", Projects.Select(p => p.Title))
            : "--";

        [TableColumn(DbColumn = "Projects",
            DbFuncFormat = "{0}.Any(p=> <condition>)<sel>p.Category.Name", Orderable = ServerClient.None)]
        [DisplayName("Categories")]
        
        public string ProjectCategories => Projects?.Count is { } nr && nr > 0
            ? string.Join(", ", Projects.Select(p => p.Category.Name))
            : "--";

        public override string ToString()
        {
            return FullName;
        }
    }
}