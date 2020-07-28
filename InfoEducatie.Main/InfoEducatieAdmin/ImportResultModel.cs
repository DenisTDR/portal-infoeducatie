using System.Collections.Generic;

namespace InfoEducatie.Main.InfoEducatieAdmin
{
    public class ImportResultModel
    {
        public int Rows { get; set; }
        public int Added { get; set; }
        public int Updated { get; set; }
        public int NotTouched { get; set; }
        public int ErrorCount { get; set; }
        public int ProjectLinkAdded { get; set; }
        public List<string> Errors { get; } = new List<string>();

        public void Add(ImportResultModel model)
        {
            Rows += model.Rows;
            Added += model.Added;
            Updated += model.Updated;
            ErrorCount += model.ErrorCount;
            Errors.AddRange(model.Errors);
        }
    }
}