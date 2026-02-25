using Microsoft.Extensions.DependencyInjection;

class Program
{
    static async Task Main(string[] args)
    {
        var services = new ServiceCollection();

        services.AddSingleton<IExcelService, ExcelService>();
        services.AddHttpClient<IFileDownloadService, PdfDownloadService>();
        services.AddSingleton<IStorageService, PdfStorageService>();
        services.AddSingleton<IPdfDownloadCoordinator, PdfDownloadCoordinator>();

        var serviceProvider = services.BuildServiceProvider();

        var excelService = serviceProvider.GetRequiredService<IExcelService>();
        var pdfCoordinator = serviceProvider.GetRequiredService<IPdfDownloadCoordinator>();

        var rapports = excelService.ReadLinks("GRI_2017_2020.xlsx", 10);

        List<DownloadResult> downloadResults = new List<DownloadResult>();

        foreach (var rapport in rapports)
        {
            try
            {
                Uri uri = await pdfCoordinator.DownloadAndSaveFileAsync(rapport, CancellationToken.None);

                downloadResults.Add(new DownloadResult
                {
                    FileName = rapport.FileName,
                    Uri = uri,
                    Success = true
                });
            }
            catch (HttpRequestException)
            {
                downloadResults.Add(new DownloadResult
                {
                    FileName = rapport.FileName,
                    Uri = null,
                    Success = false
                });
            }
        }
    }
}