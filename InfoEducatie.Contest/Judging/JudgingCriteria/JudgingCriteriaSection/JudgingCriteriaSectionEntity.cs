using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using InfoEducatie.Contest.Categories;
using MCMS.Base.Data.Entities;

namespace InfoEducatie.Contest.Judging.JudgingCriteria.JudgingCriteriaSection
{
    [Table("JudgingCriteriaSections")]
    public class JudgingCriteriaSectionEntity : Entity
    {
        [Required] public CategoryEntity Category { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            if (Category == null)
            {
                return Name;
            }

            return Category.Name + ": " + Name;
        }
    }
}