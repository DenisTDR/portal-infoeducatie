using ClosedXML.Excel;

namespace InfoEducatie.Contest.Exports;

public class ExportHelper
{
    public static void PutPageHeader(IXLWorksheet worksheet)
    {
        worksheet.Cell("A1").Value = "Ministerul Educației Naționale";
        worksheet.Cell("A2").Value = "Inspectoratul Școlar Județean Vrancea";
        worksheet.Cell("A3").Value = "InfoEducație - Olimpiada de inovare și creație digitală";
        worksheet.Cell("A4").Value = "Ediția 2025, Etapa Națională";
        worksheet.Cell("A5").Value = "Focșani 28 iulie - 1 august 2025";
        worksheet.Range("A1:A5").Style.Font.Bold = true;
    }
}