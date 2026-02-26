using Downloader.shared;
using ClosedXML.Excel;

public class ClosedXMLReportSource : IReportSource
{
    public IReadOnlyCollection<Report> ReadLinks(string path, int? rows)
    {
        using var wb = new XLWorkbook(path);

        var ws = wb.Worksheet(1);

        int fileNameCol = ExcelColumns.FileName.ColumnNumber();
        int pdfUrlCol = ExcelColumns.PdfUrl.ColumnNumber();
        int htmlUrlCol = ExcelColumns.ReportHtmlUrl.ColumnNumber();

        var lastRow = rows ?? ws.LastRowUsed()?.RowNumber() ?? 0;
        var links = new List<Report>(lastRow);

        for (int row = 2; row <= lastRow; row++)
        {
            var xlRow = ws.Row(row);

            var filename = xlRow.Cell(fileNameCol).GetString().Trim();
            var pdfUrl = xlRow.Cell(pdfUrlCol).GetString().Trim();
            var reportHtmlUrl = xlRow.Cell(htmlUrlCol).GetString().Trim();

            links.Add(new Report
            {
                FileName = filename,
                PdfUri = FileHelpers.ConvertUrlToUri(pdfUrl),
                ReportHtmlUri = FileHelpers.ConvertUrlToUri(reportHtmlUrl)
            });
        }

        return links;
    }
}