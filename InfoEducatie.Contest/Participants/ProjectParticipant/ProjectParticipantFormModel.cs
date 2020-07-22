using InfoEducatie.Contest.Participants.Participant;
using InfoEducatie.Contest.Participants.Project;
using MCMS.Base.Data.FormModels;
using MCMS.Base.SwaggerFormly.Formly.Fields;

namespace InfoEducatie.Contest.Participants.ProjectParticipant
{
    public class ProjectParticipantFormModel : IFormModel
    {
        [FormlySelect(typeof(ProjectsAdminApiController), nameof(ProjectsAdminApiController.IndexLight),
            labelProp: "title")]
        public ProjectViewModel Project { get; set; }

        [FormlySelect(typeof(ParticipantsAdminApiController), nameof(ParticipantsAdminApiController.IndexLight),
            labelProp: "fullName")]
        public ParticipantViewModel Participant { get; set; }
    }
}