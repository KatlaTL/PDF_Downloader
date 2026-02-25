using PdfDownloader.Utils;
using ClosedXML.Excel;

public class ClosedXMLService : IExcelService
{
    public List<Rapport> ReadLinks(string path, int? rows = null)
    {
        using var wb = new XLWorkbook(path);

        var ws = wb.Worksheet(1);

        int fileNameCol = ExcelColumns.FileName.ColumnNumber();
        int pdfUrlCol = ExcelColumns.PdfUrl.ColumnNumber();
        int htmlUrlCol = ExcelColumns.ReportHtmlUrl.ColumnNumber();

        var lastRow = rows ?? ws.LastRowUsed()?.RowNumber() ?? 0;
        var links = new List<Rapport>(lastRow);

        for (int row = 2; row <= lastRow; row++)
        {
            var xlRow = ws.Row(row);

            var filename = xlRow.Cell(fileNameCol).GetString().Trim();
            var pdfUrl = xlRow.Cell(pdfUrlCol).GetString().Trim();
            var reportHtmlUrl = xlRow.Cell(htmlUrlCol).GetString().Trim();

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
}