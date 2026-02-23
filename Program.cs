// See https://aka.ms/new-console-template for more information
var ExcelService = new ExcelService();

var rapports = ExcelService.ReadLinks("GRI_2017_2020.xlsx", 10);

var PdfDownloaderService = new PdfDownloaderService();

List<DownloadResult> downloadResults = new List<DownloadResult>();

foreach (var rapport in rapports)
{
    try
    {
        var url = await PdfDownloaderService.DownloadFileAsync(rapport, CancellationToken.None);

        downloadResults.Add(new DownloadResult
        {
            FileName = rapport.FileName,
            Url = url,
            Success = true
        });
    }
    catch (HttpRequestException)
    {
        downloadResults.Add(new DownloadResult
        {
            FileName = rapport.FileName,
            Url = string.Empty,
            Success = false
        });
    }
}
