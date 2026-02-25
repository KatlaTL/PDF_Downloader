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
    public async Task<Uri> DownloadAndSaveFileAsync(Rapport pdf, CancellationToken ct)
    {
        var urisToTry = new[] { pdf.PdfUri, pdf.ReportHtmlUri };
        foreach (var uri in urisToTry)
        {
            if (uri == null) continue;

            try
            {
                // Check if the uri is HTTP or HTTPS 
                if (FileHelpers.IsHttpOrHttps(uri))
                {
                    using Stream stream = await _pdfDownLoadService.DownloadAsync(uri, ct); 

                    await _pdfStorageService.SaveAsync(pdf.FileName, stream, ct);

                    return uri; // PDF downloaded
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

        throw new HttpRequestException($"Failed to download PDF for {pdf.FileName}");
    }
}