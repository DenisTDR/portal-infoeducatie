using System.ComponentModel;
using InfoEducatie.Contest.Participants.Project;
using MCMS.Base.Data.ViewModels;
using MCMS.Base.Display.ModelDisplay.Attributes;

namespace InfoEducatie.Contest.Participants.Participant
{
    [DisplayName("Participant")]
    public class ParticipantViewModel : ViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [TableColumn] public string FullName => FirstName != null ? FirstName + " " + LastName : PhoneNumber;
        public string PhoneNumber { get; set; }
        [TableColumn] public int Grade { get; set; }
        [TableColumn] public string City { get; set; }
        [TableColumn] public string County { get; set; }
        public string Country { get; set; }
        public string School { get; set; }
        public string SchoolCity { get; set; }
        public string SchoolCounty { get; set; }
        public string SchoolCountry { get; set; }
        public string MentoringTeacher { get; set; }
        public ProjectViewModel Project { get; set; }
        public string ProjectName => Project?.Title ?? "invalid";

        public override string ToString()
        {
            return FullName;
        }
    }
}