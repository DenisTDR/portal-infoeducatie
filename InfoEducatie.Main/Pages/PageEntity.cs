using System.ComponentModel.DataAnnotations.Schema;
using MCMS.Base.Data.Entities;

namespace InfoEducatie.Main.Pages
{
    [Table("BasicPages")]
    public class PageEntity : Entity, ISluggable, IOrderable, IPublishable
    {
        public string Slug { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string Body { get; set; }
        public int Order { get; set; }

        public override string ToString()
        {
            return Title;
        }

        public bool Published { get; set; }
    }
}