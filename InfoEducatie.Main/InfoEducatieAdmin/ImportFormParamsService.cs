using MCMS.SwaggerFormly.FormParamsHelpers;
using MCMS.SwaggerFormly.Models;
using Microsoft.AspNetCore.Mvc;

namespace InfoEducatie.Main.InfoEducatieAdmin
{
    public class ImportFormParamsService : FormParamsService
    {
        public ImportFormParamsService(IUrlHelper urlHelper) : base(urlHelper, "InfoEducatieAdminApi",
            "ImportFormModel")
        {
        }

        protected override string GetActionName(FormActionType actionType)
        {
            return "Import";
        }
    }
}