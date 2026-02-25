using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
class Program
{
    static async Task Main(string[] args)
    {
        var services = new ServiceCollection();

        services.AddHttpClient<IFileDownloadService, PdfDownloadService>();
        services.AddSingleton<IFilePathProvider, DefaultFilePathProvider>();
        services.AddSingleton<IExcelService, ClosedXMLService>();
        services.AddSingleton<IStorageService, PdfStorageService>();
        services.AddSingleton<IPdfDownloadCoordinator, PdfDownloadCoordinator>();

        using var serviceProvider = services.BuildServiceProvider();

        var excelService = serviceProvider.GetRequiredService<IExcelService>();
        var pdfCoordinator = serviceProvider.GetRequiredService<IPdfDownloadCoordinator>();


        var sw = Stopwatch.StartNew();

        var rapports = excelService.ReadLinks("GRI_2017_2020.xlsx", 10);

        List<DownloadResult> downloadResults;

        downloadResults = await pdfCoordinator.DownloadAndSaveFilesAsync(rapports, CancellationToken.None, 20);

        excelService.ExportToExcel(downloadResults);


        sw.Stop();
        var process = Process.GetCurrentProcess();
        var memMB = GC.GetTotalMemory(false) / 1024 / 1024;
        Console.WriteLine($"Memory: {process.WorkingSet64 / 1024 / 1024} MB");
        Console.WriteLine($"Time: {sw.Elapsed}, Memory: {memMB} MB");
    }
}