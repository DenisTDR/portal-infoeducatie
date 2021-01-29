using System.ComponentModel;
using MCMS.Base.Data.ViewModels;
using MCMS.Base.Display.ModelDisplay;
using MCMS.Base.Display.ModelDisplay.Attributes;

namespace InfoEducatie.Main.Pages
{
    [DisplayName("Basic page")]
    public class PageViewModel : ViewModel
    {
        [TableColumn] public string Slug { get; set; }

        [TableColumn] public string Title { get; set; }

        [TableColumn] public string ShortDescription { get; set; }
        public string Body { get; set; }

        [DetailsField(Hidden = true)]
        [TableColumn]
        public int Order { get; set; }

        public bool Published { get; set; }
        public bool DisplayTitle { get; set; }


        [TableColumn(Orderable = ServerClient.Client, Searchable = ServerClient.Client)]
        public string Link => Published ? $"<a target='_blank' href='/Pages/{Slug}'>Link</a>" : "Not published";

        public override string ToString()
        {
            return Title;
        }
    }
}