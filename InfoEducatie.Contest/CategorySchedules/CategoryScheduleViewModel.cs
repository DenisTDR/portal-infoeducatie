using InfoEducatie.Contest.Categories;
using MCMS.Base.Attributes.JsonConverters;
using MCMS.Base.Data.ViewModels;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace InfoEducatie.Contest.CategorySchedules;
[Display(Name = "CategorySchedule")]
public class CategoryScheduleViewModel : ViewModel
{
    [JsonConverter(typeof(ToStringJsonConverter))]
    public CategoryViewModel Category { get; set; }
    public string JsonData { get; set; }
    public string Edition { get; set; }

    public override string ToString()
    {
        return Id;
    }
}