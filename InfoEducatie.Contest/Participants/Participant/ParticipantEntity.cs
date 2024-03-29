using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using InfoEducatie.Contest.Participants.Project;
using MCMS.Base.Auth;
using MCMS.Base.Data.Entities;

namespace InfoEducatie.Contest.Participants.Participant
{
    [Table("Participants")]
    public class ParticipantEntity : Entity, ICanBeDeleted
    {
        public User User { get; set; }

        [NotMapped] public string Email => User.Email;
        
        [NotMapped]
        public string FirstName
        {
            get => User?.FirstName;
            set
            {
                if (User != null)
                    User.FirstName = value;
            }
        }

        [NotMapped]
        public string LastName
        {
            get => User?.LastName;
            set
            {
                if (User != null)
                    User.LastName = value;
            }
        }

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

        public List<ProjectEntity> Projects { get; set; }


        public override string ToString()
        {
            return User?.ToString() ?? PhoneNumber;
        }

        public bool Deleted { get; set; }
    }
}