using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using MCMS.Base.Attributes;
using MCMS.Controllers.Api;
using MCMS.Data;
using MCMS.Files.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace InfoEducatie.Main.InfoEducatieAdmin
{
    [Authorize(Roles = "Admin")]
    public class InfoEducatieAdminApiController : AdminApiController
    {
        [HttpPost]
        [ModelValidation]
        public async Task<IActionResult> Import([FromBody] [Required] ImportFormModel model)
        {
            var filesRepo = ServiceProvider.GetService<IRepository<FileEntity>>();
            var contestantsFile = await filesRepo.GetOneOrThrow(model.ContestantsFile.Id);
            var projectsFile = await filesRepo.GetOneOrThrow(model.ProjectsFile.Id);

            var result = await ServiceProvider.GetService<ImportService>().Import(projectsFile, contestantsFile, model.JustProcessAndDebug);

            // await filesRepo.Delete(contestantsFile);
            // await filesRepo.Delete(projectsFile);

            return Ok(result);
        }
    }
}