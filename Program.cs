using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
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






        var rapports = excelService.ReadLinks("GRI_2017_2020.xlsx", 100);

        List<DownloadResult> downloadResults = new List<DownloadResult>();

        var sw = Stopwatch.StartNew();
        downloadResults = await pdfCoordinator.DownloadAndSaveFilesAsync(rapports, CancellationToken.None, 20);
        /* foreach (var rapport in rapports)
        {
            downloadResults.Add(await pdfCoordinator.DownloadAndSaveFileAsync(rapport, CancellationToken.None));
        } */

        sw.Stop();
        var process = Process.GetCurrentProcess();
        var memMB = GC.GetTotalMemory(false) / 1024 / 1024;
        Console.WriteLine($"Memory: {process.WorkingSet64 / 1024 / 1024} MB");
        Console.WriteLine($"Time: {sw.Elapsed}, Memory: {memMB} MB");
    }
}