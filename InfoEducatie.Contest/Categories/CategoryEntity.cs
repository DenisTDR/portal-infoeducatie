using System.ComponentModel.DataAnnotations.Schema;
using MCMS.Base.Data.Entities;

namespace InfoEducatie.Contest.Categories
{
    [Table("Categories")]
    public class CategoryEntity : Entity, ISluggable, IOrderable, IPublishable
    {
        public string Name { get; set; }
        public string Slug { get; set; }
        public int Order { get; set; }
        public bool Published { get; set; }

        public bool ScoresX10 { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}