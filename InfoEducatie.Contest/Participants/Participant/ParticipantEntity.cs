using System.ComponentModel.DataAnnotations.Schema;
using InfoEducatie.Contest.Participants.Project;
using MCMS.Base.Auth;
using MCMS.Base.Data.Entities;

namespace InfoEducatie.Contest.Participants.Participant
{
    [Table("Participants")]
    public class ParticipantEntity : Entity
    {
        public User User { get; set; }
        [NotMapped] public string FirstName => User?.FirstName;
        [NotMapped] public string LastName => User?.LastName;
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
        public ProjectEntity Project { get; set; }

        public override string ToString()
        {
            return User?.ToString() ?? PhoneNumber;
        }
    }
}