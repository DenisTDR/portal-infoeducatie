using System.ComponentModel.DataAnnotations;
using InfoEducatie.Contest.Participants.Participant;
using InfoEducatie.Contest.Participants.Project;
using MCMS.Base.Data.FormModels;
using MCMS.Base.SwaggerFormly.Formly.Fields;

namespace InfoEducatie.Contest.Participants.ProjectParticipant
{
    public class ProjectParticipantFormModel : IFormModel
    {
        [FormlyAutocomplete(typeof(ProjectsAdminApiController), nameof(ProjectsAdminApiController.IndexLight),
            labelProp: "title")]
        [Required]
        public ProjectViewModel Project { get; set; }

        [FormlyAutocomplete(typeof(ParticipantsAdminApiController), nameof(ParticipantsAdminApiController.IndexLight),
            labelProp: "fullName")]
        [Required]
        public ParticipantViewModel Participant { get; set; }
    }
}