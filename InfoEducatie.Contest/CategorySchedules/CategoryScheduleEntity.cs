using InfoEducatie.Contest.Categories;
using MCMS.Base.Data.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace InfoEducatie.Contest.CategorySchedules;
[Table("CategorySchedules")]
public class CategoryScheduleEntity : Entity
{
    public CategoryEntity Category { get; set; }
    public string JsonData { get; set; }
    public string Edition { get; set; }

    public override string ToString()
    {
        return Id;
    }
}