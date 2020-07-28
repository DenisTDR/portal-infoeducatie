using System.Collections.Generic;
using InfoEducatie.Contest.Categories;
using InfoEducatie.Contest.Judging.Judging;

namespace InfoEducatie.Contest.Judging.Results
{
    public class DetailedResultsModel
    {
        public List<CategoryEntity> AvailableCategories { get; set; }

        public List<JudgingPageModel> JudgingPageModels { get; set; }
    }
}