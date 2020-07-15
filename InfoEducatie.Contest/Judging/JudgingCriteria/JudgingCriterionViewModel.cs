using System.ComponentModel;
using InfoEducatie.Contest.Categories;
using MCMS.Base.Data.ViewModels;
using MCMS.Base.Display.ModelDisplay.Attributes;

namespace InfoEducatie.Contest.Judging.JudgingCriteria
{
    [DisplayName("Judging Criterion")]
    public class JudgingCriterionViewModel : ViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Points { get; set; }
        public int Order { get; set; }

        [DetailsField(Hidden = true)]
        [DisplayName("Category")] public string CategoryName => Category.Name;
        [TableColumn(Hidden = true)] public CategoryViewModel Category { get; set; }

        public override string ToString()
        {
            return Name + (Category != null ? $" ({Category})" : "");
        }
    }
}