using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using InfoEducatie.Contest.Categories;
using InfoEducatie.Contest.Judging.Judges;
using InfoEducatie.Contest.Judging.JudgingCriteria;
using InfoEducatie.Contest.Participants.Project;
using MCMS.Base.Data.Entities;

namespace InfoEducatie.Contest.Judging.ProjectJudgingCriterionPoints
{
    [DisplayName("Project Judging Criterion Points")]
    public class ProjectJudgingCriterionPointsEntity : Entity
    {
        [Required] public JudgeEntity Judge { get; set; }
        [Required] public JudgingCriterionEntity Criterion { get; set; }

        [NotMapped] public CategoryEntity Categoey => Criterion.Category;
        [Required] public ProjectEntity Project { get; set; }
        public int Points { get; set; }

        public override string ToString()
        {
            return $"{Points} for {Project?.Title} from {Judge?.FullName} in {Criterion?.Name}";
        }
    }
}