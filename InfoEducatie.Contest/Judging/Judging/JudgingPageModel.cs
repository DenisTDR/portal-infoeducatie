using System.Collections.Generic;
using InfoEducatie.Contest.Categories;
using InfoEducatie.Contest.Judging.Judges;
using InfoEducatie.Contest.Judging.JudgingCriteria;
using InfoEducatie.Contest.Judging.JudgingCriteria.JudgingCriteriaSection;
using InfoEducatie.Contest.Judging.ProjectJudgingCriterionPoints;
using InfoEducatie.Contest.Participants.Project;

namespace InfoEducatie.Contest.Judging.Judging
{
    public class JudgingPageModel
    {
        public CategoryEntity Category { get; set; }
        public JudgeEntity Judge { get; set; }

        public List<ProjectViewModel> Projects { get; set; }
        // public List<JudgingCriterionViewModel> JudgingCriteria { get; set; }
        public List<JudgingCriteriaSectionViewModel> JudgingSections { get; set; }
        public List<ProjectJudgingCriterionPointsViewModel> InitialPoints { get; set; }
        
        public JudgingType Type { get; set; }
    }
}