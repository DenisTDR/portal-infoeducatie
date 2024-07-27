using System.Threading.Tasks;
using ClosedXML.Excel;
using InfoEducatie.Contest.Schedule.Models;
using MCMS.Base.Attributes;

namespace InfoEducatie.Contest.Schedule;

[Service]
public class ScheduleXlsxService
{
    public XLWorkbook BuildXlsx(ScheduleDto config)
    {
        var workbook = new XLWorkbook();

        foreach (var category in config.Categories)
        {
            var ws = workbook.AddWorksheet(category.CategoryName);
            ws.Cell("A1").Value = category.CategoryName;
            ws.Range("A1:C1").Merge();
            ws.Cell("A1").WorksheetRow().Style.Font.FontSize = 18;

            var crtRow = 3;

            foreach (var day in category.Days)
            {
                ws.Cell("A" + crtRow).Value = day.Title;
                ws.Range("A" + crtRow + ":C" + crtRow).Merge();
                ws.Cell("A" + crtRow).WorksheetRow().Style.Font.FontSize = 12;
                crtRow++;
                ws.Cell("A" + crtRow).Value = "#";
                ws.Cell("B" + crtRow).Value = "Proiect";
                ws.Cell("C" + crtRow).Value = "Ora";
                crtRow++;
                foreach (var scheduleSlotDto in day.Slots)
                {
                    if (!scheduleSlotDto.IsBreak)
                    {
                        ws.Cell("A" + crtRow).Value = scheduleSlotDto.Hashtag;
                        ws.Cell("B" + crtRow).Value = scheduleSlotDto.Title;
                    }
                    else
                    {
                        ws.Cell("A" + crtRow).Value = scheduleSlotDto.Title;
                        ws.Range("A" + crtRow + ":B" + crtRow).Merge();
                    }

                    ws.Cell("C" + crtRow).Value = scheduleSlotDto.StartTime.ToString("HH:mm") + " - " +
                                                  scheduleSlotDto.EndTime.ToString("HH:mm");
                    crtRow++;
                }

                crtRow++;
                crtRow++;
                ws.Columns("A:D").AdjustToContents();
            }
        }

        return workbook;
    }
}