using Microsoft.Extensions.Options;

public class DownloadReportsUseCase
{
    private readonly IReportSource _reportSource;
    private readonly IPdfDownloader _downloader;
    private readonly IStorageService _storage;
    private readonly IResultExporter _exporter;
    private readonly AppSettings _settings;
    public DownloadReportsUseCase(
        IReportSource reportSource,
        IPdfDownloader downloader,
        IStorageService storage,
        IResultExporter exporter,
        IOptions<AppSettings> settings)
    {
        _reportSource = reportSource;
        _downloader = downloader;
        _storage = storage;
        _exporter = exporter;
        _settings = settings.Value;
    }

    public async Task ExecuteAsync(CancellationToken ct)
    {
        var reports = _reportSource.ReadLinks(_settings.InputExcelPath, 20);

        var results = await ProcessReportsAsync(reports, ct);

        await _exporter.ExportAsync(results);
    }
    public async Task<List<DownloadResult>> ProcessReportsAsync(IEnumerable<Report> reports, CancellationToken ct)
    {
        var semaphore = new SemaphoreSlim(_settings.MaxConcurrency);

        var tasks = reports.Select(async report =>
        {
            await semaphore.WaitAsync(ct);
            try
            {
                var result = await _downloader.DownloadAsync(report, ct);

                if (result.Success && result.Stream is not null)
                {
                    await _storage.SaveAsync(report.FileName, result.Stream, ct);
                }

                return result;
            }
            finally
            {
                semaphore.Release();
            }
        });


        DownloadResult[] results = await Task.WhenAll(tasks);

        return results.ToList();
    }
}