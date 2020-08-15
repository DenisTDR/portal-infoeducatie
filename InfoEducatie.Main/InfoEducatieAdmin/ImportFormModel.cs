using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MCMS.Base.Data.FormModels;
using MCMS.Base.SwaggerFormly.Formly.Fields;
using MCMS.Files.Attributes;
using MCMS.Files.Controllers;
using MCMS.Files.Models;

namespace InfoEducatie.Main.InfoEducatieAdmin
{
    public class ImportFormModel : IFormModel
    {
        // [FormlyFieldDefaultValue(ImportSourceType.File)]
        // public ImportSourceType Type { get; set; }

        [FormlyFile(typeof(FilesAdminApiController), nameof(FilesAdminApiController.Upload), "import-participants",
            "import-participants", Private = true, Accept = new[] {"csv"})]
        [Required]
        // [FormlyFieldProp("hideExpression", "model.type !== 'file'")]
        [Display(Description = "Allowed extensions: .csv")]
        public FileFormModel ContestantsFile { get; set; }

        [FormlyFile(typeof(FilesAdminApiController), nameof(FilesAdminApiController.Upload), "import-participants",
            "import-participants", Private = true, Accept = new[] {"csv"})]
        [Required]
        // [FormlyFieldProp("hideExpression", "model.type !== 'file'")]
        [Display(Description = "Allowed extensions: .csv")]
        public FileFormModel ProjectsFile { get; set; }

        // [DataType(DataType.MultilineText)]
        // [FormlyFieldProp("hideExpression", "model.type !== 'csv'")]
        // [Required]
        // public string Csv { get; set; }

        // [Display(Name = "Fix Encoding",
        //     Description =
        //         "Read file as UTF8 string, convert this string to 1252 (encoding) bytes, convert those bytes to UTF8 string again.")]
        // [FormlyFieldDefaultValue(true)]
        // public bool FixEncoding { get; set; }
        [Display(Name = "Just process and debug",
            Description = "Don't save in db and just process and return how many entities were processed.")]
        [FormlyField(DefaultValue = true)]
        public bool JustProcessAndDebug { get; set; }
    }

    public enum ImportSourceType
    {
        [EnumMember(Value = "csv")] Csv,
        [EnumMember(Value = "file")] File
    }
}