using System.ComponentModel;
using MCMS.Base.Data.ViewModels;
using MCMS.Base.Display.ModelDisplay.Attributes;

namespace InfoEducatie.Main.Pages
{
    [DisplayName("Basic page")]
    public class PageViewModel : ViewModel
    {
        public string Slug { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        [TableColumn(Hidden = true)] public string Body { get; set; }

        [DetailsField(Hidden = true)] public int Order { get; set; }
        [TableColumn(Hidden = true)] public bool Published { get; set; }
        [TableColumn(Hidden = true)] public bool DisplayTitle { get; set; }

        public string Link => Published ? $"<a target='_blank' href='/Pages/{Slug}'>Link</a>" : "Not published";

        public override string ToString()
        {
            return Title;
        }
    }
}