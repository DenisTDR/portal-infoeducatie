using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ClosedXML.Excel;
using InfoEducatie.Contest.Categories;
using InfoEducatie.Contest.Judging.Judges;
using InfoEducatie.Contest.Judging.JudgingCriteria;
using InfoEducatie.Contest.Judging.JudgingCriteria.JudgingCriteriaSection;
using InfoEducatie.Contest.Judging.ProjectJudgingCriterionPoints;
using InfoEducatie.Contest.Participants.Project;
using MCMS.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InfoEducatie.Main.InfoEducatieAdmin
{
    public class FinalXlsxExportService
    {
        private readonly IServiceProvider _serviceProvider;

        private IRepository<ProjectEntity> ProjectsRepo => _serviceProvider.GetService<IRepository<ProjectEntity>>();
        private IRepository<JudgeEntity> JudgesRepo => _serviceProvider.GetService<IRepository<JudgeEntity>>();

        private IRepository<JudgingCriteriaSectionEntity> SectionsRepo =>
            _serviceProvider.GetService<IRepository<JudgingCriteriaSectionEntity>>();

        private IRepository<ProjectJudgingCriterionPointsEntity> GivenPointsRepo =>
            _serviceProvider.GetService<IRepository<ProjectJudgingCriterionPointsEntity>>();

        private IMapper _mapper => _serviceProvider.GetService<IMapper>();


        public FinalXlsxExportService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            JudgesRepo.ChainQueryable(q => q
                .Include(j => j.User)
                .Include(j => j.Category)
            );
            SectionsRepo.ChainQueryable(q => q.Include(j => j.Criteria));
            GivenPointsRepo.ChainQueryable(q => q
                .Include(gp => gp.Judge)
                .Include(gp => gp.Project)
                .Include(gp => gp.Criterion)
                .ThenInclude(c => c.Section)
            );
        }

        public async Task<XLWorkbook> BuildWorkbookForCategory(CategoryEntity category)
        {
            var workbook = new XLWorkbook();
            workbook.Style.Font.FontName = "Times new roman";

            var proiects = await ProjectsRepo.GetAll(p => p.Category == category);
            var judges = await JudgesRepo.GetAll(j => j.Category == category);

            BuildProjectsSheet(workbook.Worksheets.Add("Listă proiecte"), proiects, category,
                judges.FirstOrDefault(j => j.IsVicePresident));

            #region BI

            var projectCriteriaSections =
                await SectionsRepo.GetAll(s => s.Category == category && s.Type == JudgingType.Project);

            var projectGivenPoints = await GivenPointsRepo.GetAll(gp =>
                gp.Criterion.Category == category && gp.Criterion.Type == JudgingType.Project);

            foreach (var judge in judges)
            {
                var sheetTitle = "BI - " + judge.FullName;
                sheetTitle = sheetTitle.Substring(0, Math.Min(sheetTitle.Length, 31));
                BuildBorderouIndividual(workbook.Worksheets.Add(sheetTitle), judge, proiects, projectCriteriaSections,
                    projectGivenPoints.FindAll(p => p.Judge.Id == judge.Id));
            }

            #endregion

            #region BIO

            var openCriteriaSections =
                await SectionsRepo.GetAll(s => s.Category == category && s.Type == JudgingType.Open);

            var openGivenPoints = await GivenPointsRepo.GetAll(gp =>
                gp.Criterion.Category == category && gp.Criterion.Type == JudgingType.Open);

            foreach (var judge in judges)
            {
                var sheetTitle = "BIO - " + judge.FullName;
                sheetTitle = sheetTitle.Substring(0, Math.Min(sheetTitle.Length, 31));
                BuildBorderouIndividual(workbook.Worksheets.Add(sheetTitle), judge, proiects, openCriteriaSections,
                    openGivenPoints.FindAll(p => p.Judge.Id == judge.Id), true);
            }

            #endregion


            return workbook;
        }

        private void BuildBorderouIndividual(IXLWorksheet ws, JudgeEntity judge, List<ProjectEntity> projects,
            List<JudgingCriteriaSectionEntity> sections, List<ProjectJudgingCriterionPointsEntity> points,
            bool isOpen = false)
        {
            PutPageHeader(ws);

            var th = new List<string> {"Nr. Crt.", "Denumire proiect"};
            th.AddRange(sections.Select((s, i) => "Criteriu " + (i + 1) + "\n" + s.MaxPoints + "p"));
            th.Add("TOTAL\n" + (isOpen ? "OPEN" : "PROIECT"));

            SetTitle(ws,
                "BORDEROU INDIVIDUAL" + (isOpen ? " OPEN" : "") + " SECȚIUNEA " + judge.Category.Name.ToUpper(), 7,
                th.Count);

            var tableData = new List<List<string>> {th};
            int crtRow = 9;

            foreach (var project in projects)
            {
                var sums = sections.Select(s =>
                    points.Where(p => p.Project == project && p.Criterion.Section == s).Sum(p => p.Points)).ToList();
                var list = sums.Select(s => s.ToString()).Append(sums.Sum().ToString()).Prepend(project.Title).ToList();
                tableData.Add(list);
            }

            SetTableContent(ws, ref crtRow, tableData);

            SetWhoSigns(ws, ref crtRow, "MEMBRU EVALUATOR", judge.FullName.ToUpper(), th.Count - 1);
        }

        private void BuildProjectsSheet(IXLWorksheet ws, List<ProjectEntity> projects,
            CategoryEntity category, JudgeEntity vicePresident)
        {
            PutPageHeader(ws);

            var tableData = new List<List<string>> {new List<string> {"Nr. Crt.", "Denumire proiect"}};

            tableData.AddRange(projects.Select(p => new List<string> {p.Title}));

            SetTitle(ws, "LISTA PROIECTELOR ÎNSCRISE LA SECȚIUNEA " + category.Name.ToUpper(), 7, 5);

            int crtRow = 9;

            SetTableContent(ws, ref crtRow, tableData);

            SetWhoSigns(ws, ref crtRow, "VICEPREȘEDINTE", vicePresident?.FullName.ToUpper(), 1);
        }

        private void SetTitle(IXLWorksheet ws, string title, int crtRow, int colCount)
        {
            ws.Range($"A{crtRow}:{(char) ('A' + colCount)}{crtRow}").Merge();
            ws.Cell($"A{crtRow}").Value = title;
            ws.Cell($"A{crtRow}").Style.Font.Bold = true;
            ws.Cell($"A{crtRow}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        }

        private void SetTableContent(IXLWorksheet ws, ref int crtRow, List<List<string>> tableData)
        {
            var firstTableRow = crtRow;
            var firstCol = 'A';
            var lastCol = firstCol;
            foreach (var s in tableData[0])
            {
                ws.Cell($"{lastCol}{crtRow}").Value = s;
                lastCol++;
            }

            var tableHeaderRange = $"{firstCol}{crtRow}:{(char) (lastCol - 1)}{crtRow}";
            TableHeaderStyle(ws.Range(tableHeaderRange), ws.Row(crtRow));

            crtRow++;
            var firstTableIndexRow = crtRow;

            var nrCrt = 1;
            foreach (var rowValues in tableData.Skip(1))
            {
                ws.Cell($"{firstCol}{crtRow}").Value = nrCrt++;
                lastCol = firstCol;
                lastCol++;
                foreach (var cellValue in rowValues)
                {
                    ws.Cell($"{lastCol}{crtRow}").Value = cellValue;
                    lastCol++;
                }

                crtRow++;
            }

            SetTableBorders(ws.Range($"{firstCol}{firstTableRow}:{(char) (lastCol - 1)}{crtRow - 1}"));

            ws.Columns($"{(char) (firstCol + 1)}:{lastCol}").AdjustToContents();

            ws.Cells($"{firstCol}{firstTableIndexRow}:{firstCol}{crtRow - 1}").Style.Alignment.Horizontal =
                XLAlignmentHorizontalValues.Center;
        }

        private void SetWhoSigns(IXLWorksheet ws, ref int crtRow, string function, string name, int colCount)
        {
            var firstCol = 'A';
            crtRow += 2;

            ws.Range($"{firstCol}{crtRow}:{(char) (firstCol + colCount)}{crtRow}").Merge().Style.Alignment.Horizontal =
                XLAlignmentHorizontalValues.Center;
            ws.Cell($"{firstCol}{crtRow}").Value = $"{function},";
            ws.Cell($"{firstCol}{crtRow}").Style.Font.Bold = true;
            crtRow++;
            ws.Range($"{firstCol}{crtRow}:{(char) (firstCol + colCount)}{crtRow}").Merge().Style.Alignment.Horizontal =
                XLAlignmentHorizontalValues.Center;
            ws.Cell($"{firstCol}{crtRow}").Value = name;
        }

        private void SetTableBorders(IXLRange range)
        {
            range.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
            range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        }

        private void PutPageHeader(IXLWorksheet worksheet)
        {
            worksheet.Cell("A2").Value = "InfoEducație - Olimpiada de inovare și creație digitală";
            worksheet.Cell("A3").Value = "Ediția 2020, Etapa Națională - online";
            worksheet.Cell("A4").Value = "27 iulie - 2 august 2020";
            worksheet.Range("A2:A4").Style.Font.Bold = true;
        }

        public void TableHeaderStyle(IXLRange range, IXLRow row)
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.BackgroundColor = XLColor.FromArgb(0xBFBFBF);
            range.Cells().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            range.Cells().Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            row.Height = 33;
        }
    }
}