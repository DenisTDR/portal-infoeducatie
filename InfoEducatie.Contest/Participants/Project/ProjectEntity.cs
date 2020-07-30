using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using InfoEducatie.Contest.Categories;
using InfoEducatie.Contest.Participants.Participant;
using InfoEducatie.Contest.Participants.ProjectParticipant;
using MCMS.Base.Data.Entities;

namespace InfoEducatie.Contest.Participants.Project
{
    [Table("Projects")]
    public class ProjectEntity : Entity
    {
        public CategoryEntity Category { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Technologies { get; set; }
        public string SystemRequirements { get; set; }
        public string SourceUrl { get; set; }
        public string Homepage { get; set; }
        public string OldPlatformId { get; set; }
        public string DiscourseUrl { get; set; }
        public bool IsInOpen { get; set; }

        public float ScoreProject { get; set; }
        public float ScoreOpen { get; set; }


        public List<ProjectParticipantEntity> ProjectParticipants { get; set; }
        public List<ParticipantEntity> Participants => ProjectParticipants?.Select(p => p.Participant).ToList();

        public override string ToString()
        {
            return Title;
        }
    }
}