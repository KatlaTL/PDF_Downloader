// See https://aka.ms/new-console-template for more information
var ExcelService = new ExcelService();

var links = ExcelService.ReadLinks("GRI_2017_2020.xlsx", 10);


var PdfDownloaderService = new PdfDownloaderService();
await PdfDownloaderService.DownloadFileAsync(links[1], CancellationToken.None);
