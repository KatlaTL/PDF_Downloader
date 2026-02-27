using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using Microsoft.Extensions.Hosting;
class Program
{
    static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("App"));

        builder.Services.AddSingleton<DownloadReportsUseCase>();

        builder.Services.AddHttpClient<IPdfDownloadService, PdfDownloadService>();

        builder.Services.AddSingleton<IPdfPathProvider, PdfPathProvider>();
        builder.Services.AddSingleton<IXlsxPathProvider, XlsxPathProvider>();

        builder.Services.AddSingleton<IReportSource, ClosedXMLReportSource>();
        builder.Services.AddSingleton<IResultExporter, ClosedXMLResultExporter>();

        builder.Services.AddSingleton<IStorageService, PdfStorageService>();

        var sw = Stopwatch.StartNew();

        using var serviceProvider = builder.Services.BuildServiceProvider();

        var useCase = serviceProvider.GetRequiredService<DownloadReportsUseCase>();

        await useCase.ExecuteAsync(CancellationToken.None);

        sw.Stop();
        var process = Process.GetCurrentProcess();
        var memMB = GC.GetTotalMemory(false) / 1024 / 1024;
        Console.WriteLine($"Memory: {process.WorkingSet64 / 1024 / 1024} MB");
        Console.WriteLine($"Time: {sw.Elapsed}, Memory: {memMB} MB");
    }
}