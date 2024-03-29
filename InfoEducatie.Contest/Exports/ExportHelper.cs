using ClosedXML.Excel;

namespace InfoEducatie.Contest.Exports;

public class ExportHelper
{
    public static void PutPageHeader(IXLWorksheet worksheet)
    {
        worksheet.Cell("A1").Value = "Ministerul Educației Naționale";
        worksheet.Cell("A2").Value = "Inspectoratul Școlar Județean Vrancea";
        worksheet.Cell("A3").Value = "InfoEducație - Olimpiada de inovare și creație digitală";
        worksheet.Cell("A4").Value = "Ediția 2023, Etapa Națională";
        worksheet.Cell("A5").Value = "Focșani 24 - 29 iulie 2023";
        worksheet.Range("A1:A5").Style.Font.Bold = true;
    }
}