using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using InfoEducatie.Contest.Categories;
using InfoEducatie.Contest.Judging.JudgingCriteria;
using MCMS.Base.Auth;
using MCMS.Base.Data.Entities;

namespace InfoEducatie.Contest.Judging.Judge
{
    [Table("Judges")]
    public class JudgeEntity : Entity
    {
        public User User { get; set; }
        [Required] public CategoryEntity Category { get; set; }
        [NotMapped] public string FullName => User?.FullName ?? "invalid";
        public JudgeType AvailableFor { get; set; }

        public override string ToString()
        {
            return FullName;
        }
    }
}