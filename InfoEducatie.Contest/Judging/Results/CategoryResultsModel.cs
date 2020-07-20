using System.Collections.Generic;

namespace InfoEducatie.Contest.Judging.Results
{
    public class CategoryResultsModel
    {
        public string CategorySlug { get; set; }
        public string CategoryName { get; set; }
        public bool IncludeOpen { get; set; }
        public int ProjectJudgeCount { get; set; }
        public int OpenJudgeCount { get; set; }
        public List<ProjectResultsModel> Projects { get; set; }
    }
}