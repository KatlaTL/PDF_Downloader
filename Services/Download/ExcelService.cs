using PdfDownloader.Utils;
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
            var filename = GetCellValue(ws, row, ExcelColumns.FileName);
            var pdfUrl = GetCellValue(ws, row, ExcelColumns.PdfUrl);
            var reportHtmlUrl = GetCellValue(ws, row, ExcelColumns.ReportHtmlUrl);

            Uri? pdfUri = FileHelpers.ConvertUrlToUri(pdfUrl);

            if (pdfUri == null) continue;

            links.Add(new Rapport
            {
                FileName = filename,
                PdfUri = pdfUri,
                ReportHtmlUri = FileHelpers.ConvertUrlToUri(reportHtmlUrl)
            });
        }
        
        return links;
    }

    private string GetCellValue(IXLWorksheet ws, int row, ExcelColumns enumCol) => ws.Cell(row, enumCol.ColumnNumber()).GetString();
}