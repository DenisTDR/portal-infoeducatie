using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using InfoEducatie.Contest.Categories;
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

        public override string ToString()
        {
            return FullName;
        }
    }
}