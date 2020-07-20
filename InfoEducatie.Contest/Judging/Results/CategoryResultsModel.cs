using System.Collections.Generic;

namespace InfoEducatie.Contest.Judging.Results
{
    public class CategoryResultsModel
    {
        public string CategoryName { get; set; }
        public bool IncludeOpen { get; set; }
        public List<ProjectResultsModel> Projects { get; set; }
    }
}