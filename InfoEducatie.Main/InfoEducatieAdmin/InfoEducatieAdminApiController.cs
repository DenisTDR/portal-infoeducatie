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

            // var str = model.Csv;
            // if (model.Type == ImportSourceType.File)
            // {
            //     var filesRepo = ServiceProvider.GetService<IRepository<FileEntity>>();
            //     var file = await filesRepo.GetOneOrThrow(model.File.Id);
            //     await using var fs = new FileStream(file.PhysicalFullPath, FileMode.Open);
            //     using var sr = new StreamReader(fs, Encoding.UTF8);
            //     str = await sr.ReadToEndAsync();
            //     await filesRepo.Delete(file);
            // }
            //
            // // Console.WriteLine("importing from " + str.Length + " length csv string.");
            // var importResult = await ServiceProvider.GetService<ImportService>()
            //     .ImportParticipantsCsv(str, model.FixEncoding);
            //

            // return Ok(importResult);
            // return Ok();
        }
    }
}