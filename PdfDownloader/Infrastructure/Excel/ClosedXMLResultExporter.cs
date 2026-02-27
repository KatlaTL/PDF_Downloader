using ClosedXML.Excel;

public class ClosedXMLResultExporter : IResultExporter
{
    private readonly IXlsxPathProvider _pathProvider;
    public ClosedXMLResultExporter(IXlsxPathProvider pathProvider)
    {
        _pathProvider = pathProvider;
    }

    public async Task ExportAsync(IEnumerable<DownloadResult> results)
    {
        using var wb = new XLWorkbook();

        var ws = wb.AddWorksheet("DownloadResults");

        var tableData = results.Select(r => new
        {
            r.FileName,
            r.Uri,
            r.Status
        }).ToList();

        ws.Cell(1, 1).InsertTable(tableData);

        wb.SaveAs(_pathProvider.GetDestinationPath("DownloadResults"));
    }
}