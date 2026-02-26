using PdfDownloader.Utils;
public class PdfDownloadCoordinator : IPdfDownloadCoordinator
{
    private readonly IFileDownloadService _pdfDownLoadService;
    private readonly IStorageService _pdfStorageService;
    public PdfDownloadCoordinator(IFileDownloadService pdfDownLoadService, IStorageService pdfStorageService)
    {
        _pdfDownLoadService = pdfDownLoadService;
        _pdfStorageService = pdfStorageService;
    }

    public async Task<List<DownloadResult>> DownloadAndSaveFilesAsync(List<Rapport> rapports, CancellationToken ct, int maxConcurrency = 10)
    {
        if (rapports is not { Count: > 0 })
        {
            return [];
        }

        var semaphore = new SemaphoreSlim(maxConcurrency);

        var tasks = rapports.Select(async rapport =>
        {
            await semaphore.WaitAsync(ct);
            try
            {
                return await DownloadAndSaveFileAsync(rapport, ct);
            }
            finally
            {
                semaphore.Release();
            }
        });


        DownloadResult[] results = await Task.WhenAll(tasks);

        return results.ToList();
    }
    public async Task<DownloadResult> DownloadAndSaveFileAsync(Rapport rapport, CancellationToken ct)
    {
        var urisToTry = new[] { rapport.PdfUri, rapport.ReportHtmlUri };
        foreach (var uri in urisToTry)
        {
            if (uri == null) continue;

            try
            {
                // Check if the uri is HTTP or HTTPS 
                if (FileHelpers.IsHttpOrHttps(uri))
                {
                    using Stream stream = await _pdfDownLoadService.DownloadAsyncWithRetry(uri, ct);

                    await _pdfStorageService.SaveAsync(rapport.FileName, stream, ct);

                    // PDF downloaded
                    return new DownloadResult
                    {
                        FileName = rapport.FileName,
                        Uri = uri,
                        Success = true
                    };
                }
                else
                {
                    throw new ArgumentException("Only HTTP and HTTPS URLs are allowed.", uri.AbsoluteUri);
                }
            }
            catch (OperationCanceledException)
            {
                throw; // respect cancellation
            }
            catch (Exception ex)
            {
                // Log the exception for further investigation
                Console.WriteLine($"Error downloading file: {ex.Message}");
            }
        }

        // Download failed
        return new DownloadResult
        {
            FileName = rapport.FileName,
            Uri = rapport.PdfUri ?? rapport.ReportHtmlUri,
            Success = false
        };
    }
}