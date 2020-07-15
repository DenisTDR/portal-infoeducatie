using System;
using System.ComponentModel.DataAnnotations.Schema;
using MCMS.Base.Data.Entities;

namespace InfoEducatie.Main.Seminars
{
    [Table("Seminars")]
    public class SeminarEntity : Entity, IPublishable, ISluggable
    {
        public string Name { get; set; }
        public string Speaker { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
        public DateTime When { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public bool Published { get; set; }
        public string Slug { get; set; }
    }
}