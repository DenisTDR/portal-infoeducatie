using System.ComponentModel;
using MCMS.Base.Data.ViewModels;
using MCMS.Base.Display.ModelDisplay.Attributes;

namespace InfoEducatie.Main.Seminars
{
    [DisplayName("Seminar")]
    public class SeminarViewModel : ViewModel
    {
        public string Name { get; set; }
        public string Speaker { get; set; }
        public string ShortDescription { get; set; }
        [TableColumn(Hidden = true)] public string Description { get; set; }
        public string Link { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}