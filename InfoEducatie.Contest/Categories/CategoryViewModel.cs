using System.ComponentModel;
using MCMS.Base.Data.ViewModels;

namespace InfoEducatie.Contest.Categories
{
    [DisplayName("Category")]
    public class CategoryViewModel : ViewModel
    {
        public string Name { get; set; }
        public string Slug { get; set; }
        public int Order { get; set; }
        public bool Published { get; set; }
    }
}