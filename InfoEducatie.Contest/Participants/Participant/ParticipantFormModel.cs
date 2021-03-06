using MCMS.Base.Data.FormModels;

namespace InfoEducatie.Contest.Participants.Participant
{
    public class ParticipantFormModel : IFormModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public int Grade { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public string Country { get; set; }
        public string School { get; set; }
        public string SchoolCity { get; set; }
        public string SchoolCounty { get; set; }
        public string SchoolCountry { get; set; }
        public string MentoringTeacher { get; set; }
        public string Cnp { get; set; }
        public string IdCardSeries { get; set; }
        public string IdCardNumber { get; set; }
        public string OldPlatformId { get; set; }
        public bool ActivationEmailSent { get; set; }
                
        public SentMailsState SentMails { get; set; } 

    }
}