using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using InfoEducatie.Contest.Categories;
using MCMS.Base.Data.Entities;

namespace InfoEducatie.Contest.Judging.JudgingCriteria
{
    [Table("JudgingCriteria")]
    public class JudgingCriterionEntity : Entity, IOrderable
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Points { get; set; }
        public int Order { get; set; }
        [Required]
        public CategoryEntity Category { get; set; }

        public override string ToString()
        {
            return Name + (Category != null ? $" ({Category})" : "");
        }
    }
}