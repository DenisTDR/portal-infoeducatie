using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using InfoEducatie.Contest.Participants.Participant;
using InfoEducatie.Contest.Participants.Project;
using MCMS.Base.Data.Entities;

namespace InfoEducatie.Contest.Participants.ProjectParticipant
{
    [Table("ProjectParticipants")]
    public class ProjectParticipantEntity : Entity
    {
        [Required] public ProjectEntity Project { get; set; }
        [Required] public ParticipantEntity Participant { get; set; }

        public override string ToString()
        {
            return Project + "->" + Participant;
        }
    }
}