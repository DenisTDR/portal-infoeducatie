using System.ComponentModel.DataAnnotations;
using InfoEducatie.Contest.Categories;
using MCMS.Base.Data.FormModels;
using MCMS.Base.SwaggerFormly.Formly.Fields;

namespace InfoEducatie.Contest.CategorySchedules;

public class CategoryScheduleFormModel : IFormModel
{
    [FormlySelect(typeof(CategoriesAdminApiController), labelProp: "name", ShowReloadButton = true)]
    public CategoryViewModel Category { get; set; }

    [DataType(DataType.MultilineText)] public string JsonData { get; set; }
    public string Edition { get; set; }
}