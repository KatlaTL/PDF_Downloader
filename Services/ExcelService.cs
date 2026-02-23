
using ClosedXML.Excel;

public class ExcelService : IExcelService
{
    public List<Rapport> ReadLinks(string path, int? rows = null)
    {
        var links = new List<Rapport>();
        using var wb = new XLWorkbook(path);

        var ws = wb.Worksheet(1);

        var lastRow = rows ?? ws.LastRowUsed()?.RowNumber() ?? 0;

        for (int row = 2; row <= lastRow; row++)
        {
            var filename = getCellValue(ws, row, ExcelColumns.FileName);
            var pdfUrl = getCellValue(ws, row, ExcelColumns.PdfUrl);
            var reportHtmlUrl = getCellValue(ws, row, ExcelColumns.ReportHtmlUrl);

            links.Add(new Rapport
            {
                FileName = filename,
                PdfUrl = pdfUrl,
                ReportHtmlUrl = string.IsNullOrWhiteSpace(reportHtmlUrl) ? null : reportHtmlUrl
            });
        }
        
        return links;
    }

    private string getCellValue(IXLWorksheet ws, int row, ExcelColumns enumCol) => ws.Cell(row, enumCol.ColumnNumber()).GetString();
}