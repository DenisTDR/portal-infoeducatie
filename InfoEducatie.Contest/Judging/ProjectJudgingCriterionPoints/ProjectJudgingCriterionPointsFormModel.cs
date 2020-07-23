using System.ComponentModel.DataAnnotations;
using InfoEducatie.Contest.Judging.Judges;
using InfoEducatie.Contest.Judging.JudgingCriteria;
using InfoEducatie.Contest.Participants.Project;
using MCMS.Base.Data.FormModels;
using MCMS.Base.SwaggerFormly.Formly;
using MCMS.Base.SwaggerFormly.Formly.Fields;

namespace InfoEducatie.Contest.Judging.ProjectJudgingCriterionPoints
{
    public class ProjectJudgingCriterionPointsFormModel : IFormModel
    {
        [FormlySelect(typeof(JudgesAdminApiController), nameof(JudgesAdminApiController.IndexLight),
            labelProp: "fullName")]
        [Required]
        [DisablePatchSubProperties]
        public JudgeViewModel Judge { get; set; }

        [FormlySelect(typeof(JudgingCriteriaAdminApiController), nameof(JudgingCriteriaAdminApiController.IndexLight))]
        [Required]
        [DisablePatchSubProperties]
        public JudgingCriterionViewModel Criterion { get; set; }

        [FormlySelect(typeof(ProjectsAdminApiController), nameof(ProjectsAdminApiController.IndexLight),
            labelProp: "title")]
        [Required]
        [DisablePatchSubProperties]
        public ProjectViewModel Project { get; set; }

        public int Points { get; set; }
    }
}