using System;
using System.ComponentModel;
using MCMS.Base.Data.ViewModels;
using MCMS.Base.Display.ModelDisplay;
using MCMS.Base.Display.ModelDisplay.Attributes;

namespace InfoEducatie.Main.Seminars
{
    [DisplayName("Seminar")]
    public class SeminarViewModel : ViewModel
    {
        [TableColumn] public string Name { get; set; }
        [TableColumn] public string Speaker { get; set; }
        [TableColumn] public string ShortDescription { get; set; }
        [TableColumn(Hidden = true)] public string Description { get; set; }

        [TableColumn(Hidden = true)]
        [DetailsField(Hidden = true)]
        public string Link { get; set; }

        [TableColumn(Hidden = true)]
        [DetailsField(Hidden = true)]
        public DateTime When { get; set; }

        [TableColumn(Searchable = ServerClient.Client, DbColumn = "When")]
        [DisplayName("When")]
        public string DisplayTime => When.ToString("u");

        [TableColumn(Searchable = ServerClient.Client, Orderable = ServerClient.Client)]
        [DisplayName("Link")]
        public string DisplayLink => $"<a target='_blank' href='{Link}'>Link</a>";


        [TableColumn] public bool Published { get; set; }

        [TableColumn(Hidden = true)] public string Slug { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}