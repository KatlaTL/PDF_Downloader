
public class PdfDownloaderService : IPdfDownloaderService
{
    private static readonly HttpClient _httpClient = new HttpClient();
    public async Task DownloadFileAsync(Rapport pdf, CancellationToken ct)
    {
        var urlsToTry = new[] { pdf.PdfUrl, pdf.ReportHtmlUrl };
        foreach (var url in urlsToTry.Where(url => !string.IsNullOrWhiteSpace(url)))
        {
            try
            {
                using var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, ct);

                if (!response.IsSuccessStatusCode) continue;

                if (response.Content.Headers.ContentType?.MediaType != "application/pdf") continue;


                string destinationPath = Path.Combine(Directory.GetCurrentDirectory(), "Downloads", pdf.FileName + ".pdf");

                await using var fs = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None);

                await response.Content.CopyToAsync(fs, ct);

                return; // PDF downloaded
            }
            catch (OperationCanceledException)
            {
                throw; // respect cancellation
            }
        }

        throw new HttpRequestException($"Failed to download PDF for {pdf.FileName}");
    }
}