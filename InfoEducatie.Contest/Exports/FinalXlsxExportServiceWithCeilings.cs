using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using InfoEducatie.Contest.Categories;
using InfoEducatie.Contest.Judging.Judges;
using InfoEducatie.Contest.Judging.Judging;
using InfoEducatie.Contest.Judging.JudgingCriteria;
using InfoEducatie.Contest.Judging.JudgingCriteria.JudgingCriteriaSection;
using InfoEducatie.Contest.Judging.ProjectJudgingCriterionPoints;
using InfoEducatie.Contest.Participants.Project;
using MCMS.Base.Data;
using MCMS.Base.Extensions;
using Microsoft.EntityFrameworkCore;
using MoreLinq;

// ReSharper disable StringLiteralTypo

namespace InfoEducatie.Contest.Exports
{
    public class FinalXlsxExportServiceWithCeilings
    {
        private readonly IServiceProvider _serviceProvider;
        private bool EditionWithOpen => false;
        private bool EditionHasVicePresidents => false;

        private IRepository<ProjectEntity> ProjectsRepo => _serviceProvider.GetRepo<ProjectEntity>();
        private IRepository<JudgeEntity> JudgesRepo => _serviceProvider.GetRepo<JudgeEntity>();

        private IRepository<JudgingCriteriaSectionEntity> SectionsRepo =>
            _serviceProvider.GetRepo<JudgingCriteriaSectionEntity>();

        private IRepository<ProjectJudgingCriterionPointsEntity> GivenPointsRepo =>
            _serviceProvider.GetRepo<ProjectJudgingCriterionPointsEntity>();

        public FinalXlsxExportServiceWithCeilings(IServiceProvider serviceProvider)
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
            ProjectsRepo.ChainQueryable(q => q
                .Include(p => p.Participants)
                .ThenInclude(p => p.User));
        }

        public async Task<XLWorkbook> BuildWorkbookForCategory(CategoryEntity category)
        {
            var workbook = new XLWorkbook();
            workbook.Style.Font.FontName = "Times new roman";

            var projects = await ProjectsRepo.GetAll(p => p.Category == category);
            var judges = await JudgesRepo.GetAll(j => j.Category == category);
            var allCriteriaSections = await SectionsRepo.GetAll(s => s.Category == category);
            var allPoints = await GivenPointsRepo.GetAll(p => p.Criterion.Category == category);

            var projectCriteriaSections = allCriteriaSections.FindAll(s => s.Type == JudgingType.Project);
            var projectGivenPoints = allPoints.FindAll(gp => gp.Criterion.Type == JudgingType.Project);
            var openCriteriaSections = allCriteriaSections.FindAll(s => s.Type == JudgingType.Open);
            var openGivenPoints = allPoints.FindAll(gp => gp.Criterion.Type == JudgingType.Open);
            var openProjects = projects.Where(p => p.IsInOpen).ToList();

            BuildProjectsSheet(workbook.Worksheets.Add("Listă proiecte"), projects, category,
                judges.FirstOrDefault(j => j.IsVicePresident));

            #region BI

            foreach (var judge in judges)
            {
                var sheetTitle = "BI - " + judge.FullName;
                sheetTitle = sheetTitle.Substring(0, Math.Min(sheetTitle.Length, 31));
                BuildIndividualTallySheet(workbook.Worksheets.Add(sheetTitle), judge, projects, projectCriteriaSections,
                    projectGivenPoints.FindAll(p => p.Judge.Id == judge.Id));

                if (EditionWithOpen)
                {
                    sheetTitle = "BIO - " + judge.FullName;
                    sheetTitle = sheetTitle.Substring(0, Math.Min(sheetTitle.Length, 31));
                    BuildIndividualTallySheet(workbook.Worksheets.Add(sheetTitle), judge, openProjects,
                        openCriteriaSections,
                        openGivenPoints.FindAll(p => p.Judge.Id == judge.Id), true);
                }
            }

            #endregion

            CalcTotalScore(projects, allPoints, judges, category);

            #region BFs

            projects = projects.OrderByDescending(p => p.ScoreProject).ToList();
            BuildFinalTallySheet(workbook.Worksheets.Add("Borderou final"), judges, projects, projectGivenPoints,
                category);
            if (EditionWithOpen)
            {
                openProjects = openProjects.OrderByDescending(p => p.ScoreOpen).ToList();
                BuildFinalTallySheet(workbook.Worksheets.Add("Borderou final OPEN"), judges, openProjects,
                    openGivenPoints, category, true);
            }

            #endregion


            #region RF

            if (EditionWithOpen)
            {
                projects = projects.OrderByDescending(p => p.ScoreProject).ToList();
                BuildResultsSheet(workbook.Worksheets.Add("Rezultate înainte OPEN"), projects, category,
                    judges);
                projects = projects.OrderByDescending(p => p.ScoreProject + p.ScoreOpen).ToList();
                BuildResultsSheet(workbook.Worksheets.Add("Rezultate finale"), projects, category, judges, true);
            }
            else
            {
                projects = projects.OrderByDescending(p => p.ScoreProject + p.ScoreOpen).ToList();
                BuildResultsSheet(workbook.Worksheets.Add("Rezultate finale"), projects, category, judges);
            }

            #endregion

            #region prizes maybe

            BuildPrizesSheet(workbook.Worksheets.Add("Șablon premii"), projects, category, judges);

            #endregion

            return workbook;
        }


        private void BuildPrizesSheet(IXLWorksheet ws, List<ProjectEntity> projects, CategoryEntity category,
            List<JudgeEntity> judges)
        {
            PutPageHeader(ws);
            var th = new List<string>
            {
                "Nr. Crt.", "Județul", "Nume și prenume elev", "Clasa", "Unitate de învățământ", "Localitate",
                "Profesorul/Profesorii\nÎndrumător/Îndrumători", "Denumire proiect", "Secțiunea", "Cnp", "Serie CI",
                "Număr CI", "Punctaj total", "Premiu", "Suma", "Semnătura"
            };

            SetTitle(ws, $"PREMII SECȚIUNEA {category.Name.ToUpper()}", 7, th.Count);
            var crtRow = 9;
            var firstCol = 'A';
            var tableData = new List<List<string>> {th};
            var tableDataStartsAt = crtRow + 1;

            foreach (var project in projects)
            {
                foreach (var participant in project.Participants)
                {
                    var row = new List<string>
                    {
                        participant.County, $"{participant.LastName} {participant.FirstName}",
                        $"a {ToRomanNumber(participant.Grade)}-a", participant.School, participant.City,
                        participant.MentoringTeacher, project.Title, category.Name, participant.Cnp,
                        $"=UPPER(\"{participant.IdCardSeries}\")", participant.IdCardNumber,
                        (project.ScoreProject + project.ScoreOpen).ToString(CultureInfo.InvariantCulture), "", "", ""
                    };
                    tableData.Add(row);
                }
            }

            var rangeName =
                $"{(char) (firstCol + 11)}{tableDataStartsAt}:{(char) (firstCol + 11)}{tableDataStartsAt + tableData.Count - 2}";
            ws.Range(rangeName).Style.NumberFormat.Format = "@";

            SetTableContent(ws, ref crtRow, tableData, false, true, firstCol);
            rangeName =
                $"{(char) (firstCol + 3)}{tableDataStartsAt}:{(char) (firstCol + 3)}{tableDataStartsAt + tableData.Count - 2}";
            ws.Range(rangeName).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            rangeName =
                $"{(char) (firstCol + 8)}{tableDataStartsAt}:{(char) (firstCol + 14)}{tableDataStartsAt + tableData.Count - 2}";
            var range = ws.Range(rangeName);
            range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            range.Rows().ForEach(r =>
            {
                var wsRow = r.WorksheetRow();
                wsRow.Height = 33;
                wsRow.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            });

            rangeName =
                $"{(char) (firstCol + 12)}{tableDataStartsAt}:{(char) (firstCol + 12)}{tableDataStartsAt + tableData.Count - 2}";
            ws.Range(rangeName).Style.NumberFormat.Format = "#0";


            SetWhoSigns(ws, ref crtRow,
                new Tuple<string, string>("VICEPREȘEDINTE", judges.FirstOrDefault(j => j.IsVicePresident)?.FullName),
                new Tuple<string, List<string>>("MEMBRI EVALUATORI",
                    judges.Where(j => !j.IsVicePresident).Select(j => j.FullName).ToList()), th.Count);
        }

        private void CalcTotalScore(List<ProjectEntity> projects, List<ProjectJudgingCriterionPointsEntity> points,
            List<JudgeEntity> judges, CategoryEntity cat)
        {
            foreach (var projectEntity in projects)
            {
                // projectEntity.ScoreProject =
                //     (float) Math.Ceiling(Math.Ceiling(
                //         1f * points.Where(p => p.Project == projectEntity && p.Criterion.Type == JudgingType.Project)
                //             .Sum(p => p.Points)
                //         / judges.Count) / (cat.ScoresX10 ? 10 : 1));
                projectEntity.ScoreProject =
                    (int) Math.Ceiling(
                        points.Where(p => p.Project == projectEntity && p.Criterion.Type == JudgingType.Project)
                            .GroupBy(p => p.Judge)
                            .Select(gr => Math.Ceiling(gr.Sum(p => p.Points / (cat.ScoresX10 ? 10d : 1))))
                            .Sum()
                        / judges.Count);
                // projectEntity.ScoreOpen =
                //     (float) Math.Ceiling(Math.Ceiling(
                //         1f * points.Where(p => p.Project == projectEntity && p.Criterion.Type == JudgingType.Open)
                //             .Sum(p => p.Points)
                //         / judges.Count) / (cat.ScoresX10 ? 10 : 1));
                projectEntity.ScoreOpen =
                    (int) Math.Ceiling(
                        points.Where(p => p.Project == projectEntity && p.Criterion.Type == JudgingType.Open)
                            .GroupBy(p => p.Judge)
                            .Select(gr => Math.Ceiling(gr.Sum(p => p.Points / (cat.ScoresX10 ? 10d : 1))))
                            .Sum()
                        / judges.Count);
            }
        }

        private void BuildResultsSheet(IXLWorksheet ws, List<ProjectEntity> projects, CategoryEntity category,
            List<JudgeEntity> judges, bool withOpen = false)
        {
            PutPageHeader(ws);
            var th = new List<string>
            {
                "Nr. Crt.", "Denumire proiect", "Nume și prenume elev", "Unitate de învățământ", "Localitate", "Județ",
                // "MEN", 
            };

            if (!withOpen)
            {
                th.Add("Punctaj total");
            }
            else
            {
                th.Add("Punctaj inițial");
                th.Add("Punctaj Open");
                th.Add("Punctaj Final");
            }

            SetTitle(ws, $"REZULTATE {(withOpen ? "FINALE" : "ÎNAINTE DE OPEN")} SECȚIUNEA {category.Name.ToUpper()}",
                7, th.Count);
            var crtRow = 9;
            var firstCol = 'A';
            var tableData = new List<List<string>> {th};
            var tableDataStartsAt = crtRow + 1;

            foreach (var project in projects)
            {
                foreach (var participant in project.Participants)
                {
                    var row = new List<string>
                    {
                        project.Title,
                        $"{participant.LastName} {participant.FirstName}",
                        participant.School,
                        participant.City,
                        participant.County,
                        project.ScoreProject.ToString(CultureInfo.InvariantCulture)
                    };
                    if (withOpen)
                    {
                        row.Add(project.ScoreOpen == 0 ? "" : project.ScoreOpen.ToString(CultureInfo.InvariantCulture));
                        row.Add(
                            $"=CEILING({(char) (firstCol + row.Count - 1)}<crtRow>+{(char) (firstCol + row.Count)}<crtRow>, 1)");
                    }

                    tableData.Add(row);
                }
            }

            SetTableContent(ws, ref crtRow, tableData, false, true, firstCol);

            var specialRangeName =
                $"{(char) (firstCol + 4)}{tableDataStartsAt}:{(char) (firstCol + (withOpen ? 9 : 7))}{tableDataStartsAt + tableData.Count - 2}";
            var specialRange = ws.Range(specialRangeName);
            specialRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            // specialRange.Style.NumberFormat.Format = "#0.00";
            specialRange.Rows().ForEach(r =>
            {
                var wsRow = r.WorksheetRow();
                wsRow.Height = 33;
                wsRow.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            });

            SetWhoSigns(ws, ref crtRow,
                new Tuple<string, string>("VICEPREȘEDINTE", judges.FirstOrDefault(j => j.IsVicePresident)?.FullName),
                new Tuple<string, List<string>>("MEMBRI EVALUATORI",
                    judges.Where(j => !j.IsVicePresident).Select(j => j.FullName).ToList()), th.Count);
        }

        private void BuildFinalTallySheet(IXLWorksheet ws, List<JudgeEntity> judges, List<ProjectEntity> projects,
            List<ProjectJudgingCriterionPointsEntity> points, CategoryEntity category, bool isOpen = false)
        {
            PutPageHeader(ws);
            var th = new List<string> {"Nr. Crt.", "Denumire proiect"};
            th.AddRange(judges.Select(j => "Punctaj\n" + j.FullName).Append("Media"));
            SetTitle(ws,
                $"BORDEROU{(isOpen ? " OPEN" : "")} SECȚIUNEA {category.Name.ToUpper()}", 7, th.Count);

            int crtRow = 9;

            var tableDataStartsAt = crtRow + 1;
            var firstCol = 'A';
            var tableData = new List<List<string>> {th};

            foreach (var project in projects)
            {
                var row = new List<string> {project.Title};
                row.AddRange(judges.Select(j =>
                {
                    // var sum = points.Where(p => p.Judge == j && p.Project == project).Sum(p => p.Points);
                    // return (sum / (category.ScoresX10 ? 10f : 1)).ToString();
                    // var sum = Math.Ceiling(sections.Select(s => s.CalcPointsFor(points, project, category, j)).Sum());
                    // return sum.ToString(CultureInfo.InvariantCulture);

                    var sum = points.Where(p => p.Judge == j && p.Project == project)
                        .Sum(p => Math.Round(p.Points / (category.ScoresX10 ? 10f : 1), 2));
                    return Math.Ceiling(sum).ToString(CultureInfo.InvariantCulture);
                }));
                row.Add(
                    $"=CEILING(AVERAGE({(char) (firstCol + 2)}<crtRow>:{(char) (firstCol + row.Count)}<crtRow>), 1)");
                tableData.Add(row);
            }

            SetTableContent(ws, ref crtRow, tableData, false, true, firstCol);

            var specialRangeName =
                $"{(char) (firstCol + 2)}{tableDataStartsAt}:{(char) (firstCol + th.Count - 1)}{tableDataStartsAt + tableData.Count - 2}";
            var specialRange = ws.Range(specialRangeName);
            specialRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            SetWhoSigns(ws, ref crtRow,
                new Tuple<string, string>("VICEPREȘEDINTE", judges.FirstOrDefault(j => j.IsVicePresident)?.FullName),
                new Tuple<string, List<string>>("MEMBRI EVALUATORI",
                    judges.Where(j => !j.IsVicePresident).Select(j => j.FullName).ToList()), th.Count);
        }

        private void BuildIndividualTallySheet(IXLWorksheet ws, JudgeEntity judge, List<ProjectEntity> projects,
            List<JudgingCriteriaSectionEntity> sections, List<ProjectJudgingCriterionPointsEntity> points,
            bool isOpen = false)
        {
            var cat = judge.Category;
            PutPageHeader(ws);

            var th = new List<string> {"Nr. Crt.", "Denumire proiect"};
            th.AddRange(sections.Select((s, i) =>
                "Criteriu " + (i + 1) + "\n" + s.MaxPoints / (cat.ScoresX10 ? 10f : 1) + "p"));
            th.Add("TOTAL\n" + (isOpen ? "OPEN" : "PROIECT"));

            SetTitle(ws,
                "BORDEROU INDIVIDUAL" + (isOpen ? " OPEN" : "") + " SECȚIUNEA " + judge.Category.Name.ToUpper(), 7,
                th.Count);

            var tableData = new List<List<string>> {th};
            int crtRow = 9;

            var firstCol = 'A';
            foreach (var project in projects)
            {
                // var row = sections.Select(s =>
                //     points.Where(p => p.Project == project && p.Criterion.Section == s).Sum(p => p.Points)
                //     / (cat.ScoresX10 ? 10f : 1)
                // ).Select(s => s.ToString()).ToList();
                // var row = sections.Select(s => s.CalcPointsFor(points, project, cat)).Select(s => s.ToString())
                //     .ToList();

                var row = sections.Select(s =>
                    Math.Round(points.Where(p => p.Project == project && p.Criterion.Section == s).Sum(p => p.Points) /
                               (cat.ScoresX10 ? 10f : 1), 2)
                ).Select(s => s.ToString(CultureInfo.InvariantCulture)).ToList();

                row.Insert(0, project.Title);
                row.Add($"=CEILING(SUM({(char) (firstCol + 2)}<crtRow>:{(char) (firstCol + row.Count)}<crtRow>), 1)");
                tableData.Add(row);
            }

            SetTableContent(ws, ref crtRow, tableData, true, true, firstCol);

            SetWhoSignsIndividual(ws, ref crtRow, "MEMBRU EVALUATOR", judge.FullName.ToUpper(), th.Count - 1);
        }

        private void BuildProjectsSheet(IXLWorksheet ws, List<ProjectEntity> projects,
            CategoryEntity category, JudgeEntity vicePresident)
        {
            PutPageHeader(ws);

            var tableData = new List<List<string>> {new List<string> {"Nr. Crt.", "Denumire proiect"}};

            tableData.AddRange(projects.Select(p => new List<string> {p.Title}));

            SetTitle(ws, "LISTA PROIECTELOR ÎNSCRISE LA SECȚIUNEA " + category.Name.ToUpper(), 7, 5);

            int crtRow = 9;

            SetTableContent(ws, ref crtRow, tableData, true, true);

            if (EditionHasVicePresidents)
            {
                SetWhoSignsIndividual(ws, ref crtRow, "VICEPREȘEDINTE", vicePresident?.FullName.ToUpper(), 1);
            }
        }

        public void BuildAvgScoresExceptJudgeSheet(IXLWorksheet ws, JudgingPageModel pageModel,
            List<ProjectJudgingCriterionPointsEntity> allPoints, int judgesCount)
        {
            var startRow = 3;
            var crtRow = startRow;
            var th = pageModel.Projects.Select(p => p.Title).ToList();
            th.Insert(0, "Criteria");
            th.Insert(0, "#");

            var tableData = new List<List<string>> {th};
            crtRow++;
            foreach (var section in pageModel.JudgingSections)
            {
                var row = new List<string> {section.Name};
                row.AddRange(pageModel.Projects.Select(p => ""));
                tableData.Add(row);
                var sectionRow = ws.Row(crtRow);
                sectionRow.Cells($"A{crtRow}:{(char) ('A' + th.Count - 1)}{crtRow}").Style.Fill.BackgroundColor =
                    XLColor.FromArgb(0xDFDFDF);
                sectionRow.Height = 33;
                sectionRow.Style.Font.Bold = true;
                crtRow++;
                foreach (var criterion in section.Criteria)
                {
                    row = new List<string> {criterion.Name};
                    row.AddRange(pageModel.Projects.Select(p =>
                    {
                        var sum = allPoints.Where(points =>
                            points.Criterion.Id == criterion.Id && points.Project.Id == p.Id &&
                            points.Judge.Id != pageModel.Judge.Id).Sum(p1 => p1.Points);
                        return (1f * sum / (judgesCount - 1)).ToString("#0.00");
                    }));
                    tableData.Add(row);
                    ws.Row(crtRow).Height = 33;
                    crtRow++;
                }
            }

            SetTableContent(ws, ref startRow, tableData, true);
            ws.Style.Alignment.WrapText = true;
            ws.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Column("B").Width = 33;
            ws.Row(3).Height = 75;
        }

        private static void SetTitle(IXLWorksheet ws, string title, int crtRow, int colCount)
        {
            ws.Range($"A{crtRow}:{(char) ('A' + colCount - 1)}{crtRow}").Merge();
            ws.Cell($"A{crtRow}").Value = title;
            ws.Cell($"A{crtRow}").Style.Font.Bold = true;
            ws.Cell($"A{crtRow}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        }

        private void SetTableContent(IXLWorksheet ws, ref int crtRow, List<List<string>> tableData, bool
            alignTableContentCenter = false, bool adjustColumnsWidth = false, char firstCol = (char) 0)
        {
            var firstTableRow = crtRow;
            // var firstCol = 'A';
            if (firstCol == 0)
            {
                firstCol = 'A';
            }

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
                    var celVal = cellValue ?? "--";
                    var cell = ws.Cell($"{lastCol}{crtRow}");
                    if (celVal.StartsWith("="))
                    {
                        var finalFormula = celVal.Replace("<crtRow>", crtRow.ToString());
                        cell.FormulaA1 = finalFormula;
                        // cell.Style.NumberFormat.Format = "#0.00";
                    }
                    else
                    {
                        cell.Value = celVal;
                    }

                    lastCol++;
                }

                crtRow++;
            }

            SetTableBorders(ws.Range($"{firstCol}{firstTableRow}:{(char) (lastCol - 1)}{crtRow - 1}"));

            if (adjustColumnsWidth)
            {
                ws.Columns($"{(char) (firstCol + 1)}:{lastCol}").AdjustToContents();
            }

            ws.Range($"{firstCol}{firstTableIndexRow}:{firstCol}{crtRow - 1}").Style.Alignment.Horizontal =
                XLAlignmentHorizontalValues.Center;
            if (alignTableContentCenter)
            {
                ws.Range($"{(char) (firstCol + 2)}{firstTableIndexRow}:{(char) (lastCol - 1)}{crtRow - 1}").Style
                    .Alignment
                    .Horizontal = XLAlignmentHorizontalValues.Center;
            }
        }

        private void SetWhoSignsIndividual(IXLWorksheet ws, ref int crtRow, string function, string name, int colCount)
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

        private void SetWhoSigns(IXLWorksheet ws, ref int crtRow, Tuple<string, string> boss,
            Tuple<string, List<string>> plebs, int colCount)
        {
            if (!EditionHasVicePresidents)
            {
                boss = new Tuple<string, string>("", "");
            }

            var bossCol = (char) ('A' + colCount / 3 - 1);
            var plebsCol = (char) ('A' + colCount / 3 * 2 - 1);
            if (bossCol == plebsCol)
            {
                plebsCol = (char) (bossCol + 1);
            }

            crtRow += 2;
            var bossTitleCell = ws.Cell($"{bossCol}{crtRow}");
            bossTitleCell.Value = boss.Item1.Length > 0 ? $"{boss.Item1}," : "";
            bossTitleCell.Style.Font.Bold = true;
            HCenter(bossTitleCell);
            crtRow++;
            var bossNameCell = ws.Cell($"{bossCol}{crtRow}");
            bossNameCell.Value = boss.Item2;
            HCenter(bossNameCell);

            crtRow--;
            var plebsTitleCell = ws.Cell($"{plebsCol}{crtRow}");
            plebsTitleCell.Value = $"{plebs.Item1},";
            plebsTitleCell.Style.Font.Bold = true;
            HCenter(plebsTitleCell);
            var plebNameCell = plebsTitleCell.CellBelow();
            foreach (var plebName in plebs.Item2)
            {
                plebNameCell.Value = plebName;
                HCenter(plebNameCell);
                plebNameCell = plebNameCell.CellBelow();
            }
        }

        private void HCenter(IXLCell cell)
        {
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
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

        private void TableHeaderStyle(IXLRange range, IXLRow row)
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.BackgroundColor = XLColor.FromArgb(0xBFBFBF);
            range.Cells().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            range.Cells().Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            row.Height = 33;
        }

        private static string ToRomanNumber(int nr)
        {
            var listOfNum = new[] {1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1};
            var listOfRoman = new[] {"M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I"};

            var numToRoman = "";
            for (var i = 0; i < listOfNum.Length; i++)
            {
                while (nr >= listOfNum[i])
                {
                    numToRoman += listOfRoman[i];
                    nr -= listOfNum[i];
                }
            }

            return numToRoman;
        }
    }
}