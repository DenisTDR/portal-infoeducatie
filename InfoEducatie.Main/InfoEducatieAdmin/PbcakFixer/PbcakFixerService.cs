using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;
using InfoEducatie.Contest.Participants.Participant;
using MCMS.Base.Attributes;
using MCMS.Base.Data;
using MCMS.Base.Helpers;
using Microsoft.EntityFrameworkCore;

namespace InfoEducatie.Main.InfoEducatieAdmin.PbcakFixer;

[Service]
public class PbcakFixerService(IRepository<ParticipantEntity> participantsRepo)
{
    public async Task<string> GetDiff()
    {
        var sb = new StringBuilder();
        var readParticipants = ProcessXlsx();

        // foreach (var pvm in readParticipants)
        // {
        // sb.AppendLine("{0} {1} {2} {3}", pvm.FirstName, pvm.LastName, pvm.Cnp, pvm.County);
        // }

        var existingParticipants = await participantsRepo.Query
            .Include(p => p.User)
            .Include(p => p.Projects)
            .ThenInclude(p => p.Category)
            .OrderBy(p => p.Projects.First().Category.Slug)
            .ToListAsync();

        var toFix = 0;
        foreach (var eParticipant in existingParticipants)
        {
            var correctParticipant = readParticipants.FirstOrDefault(rp => rp.Cnp == eParticipant.Cnp);
            if (correctParticipant == null) continue;

            sb.AppendLine("--------");
            sb.AppendLine(
                $"[{eParticipant.Projects.FirstOrDefault()?.Category.Slug}] {correctParticipant.Cnp}");
            sb.AppendLine(
                $"    xlsx: {correctParticipant.FirstName} {correctParticipant.LastName}  --[{correctParticipant.Grade}]-- {correctParticipant.County} - {correctParticipant.School}");
            sb.AppendLine(
                $"existing: {eParticipant.FirstName} {eParticipant.LastName}  --[{correctParticipant.Grade}]-- {eParticipant.County} - {eParticipant.School}");
            toFix++;
        }

        sb.AppendLine();
        sb.AppendLine("toFix=" + toFix);
        sb.AppendLine();
        sb.AppendLine();

        var notExisting = 0;
        foreach (var eParticipant in existingParticipants)
        {
            var correctParticipant = readParticipants.FirstOrDefault(rp => rp.Cnp == eParticipant.Cnp);
            if (correctParticipant != null) continue;

            sb.AppendLine(
                $"Existing participant not found in xlsx: {eParticipant.LastName} {eParticipant.FirstName} {eParticipant.County} {eParticipant.Cnp}");
            notExisting++;
        }

        sb.AppendLine();
        sb.AppendLine("notExisting=" + notExisting);

        return sb.ToString();
    }


    private List<ParticipantViewModel> ProcessXlsx()
    {
        var xlsxPath = Env.GetOrThrow("OFFICIAL_PARTICIPANTS_XLSX_PATH");

        var doc = new XLWorkbook(xlsxPath);
        var wb = doc.Worksheets.First();

        var list = new List<ParticipantViewModel>();

        var crtRow = 9;
        var invalidCnpCount = 0;

        while (true)
        {
            var model = new ParticipantViewModel();
            var row = wb.Row(crtRow);
            if (row.Cell("A").Value.IsBlank)
            {
                break;
            }

            model.County = row.Cell("B").Value.ToString().Trim();

            model.FirstName = row.Cell("C").Value.ToString().Trim();
            model.LastName = model.FirstName.Split(" ")[0].Trim();
            model.FirstName = model.FirstName.Replace(model.LastName, "").Trim();
            while (model.FirstName.Length > 2 && model.FirstName[1] == '.')
            {
                model.FirstName = model.FirstName[2..].Trim();
            }

            model.School = row.Cell("F").Value.ToString().Trim();
            model.City = row.Cell("G").Value.ToString().Trim();
            model.Cnp = row.Cell("H").Value.ToString().Trim();

            if (string.IsNullOrEmpty(model.Cnp))
            {
                invalidCnpCount++;
                crtRow++;
                continue;
            }

            var gradeStr = row.Cell("E").Value.ToString();
            gradeStr = gradeStr.Replace("a", "").Replace("-", "").Trim().ToUpper();
            if (gradeStr.Contains("X"))
            {
                model.Grade = new[] { "IX", "X", "XI", "XII" }.ToList().IndexOf(gradeStr);
            }
            else
            {
                model.Grade = int.Parse(gradeStr);
            }

            list.Add(model);
            crtRow++;
        }


        return list;
    }
}