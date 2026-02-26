using Downloader.shared;
public class PdfDownloader : IPdfDownloader
{
    private readonly HttpClient _httpClient;
    private readonly int _maxRetries;
    private readonly TimeSpan _delay;

    public PdfDownloader(HttpClient httpClient, int maxRetries = 3, TimeSpan? delay = null)
    {
        _httpClient = httpClient;
        _maxRetries = maxRetries;
        _delay = delay ?? TimeSpan.FromSeconds(2);
    }
    public async Task<DownloadResult> DownloadAsync(Report reports, CancellationToken ct)
    {
        var urisToTry = new[] { reports.PdfUri, reports.ReportHtmlUri };

        foreach (var uri in urisToTry)
        {
            if (uri == null) continue;

            // Check if the uri is HTTP or HTTPS 
            if (!FileHelpers.IsHttpOrHttps(uri)) continue;

            for (int attempt = 1; attempt < _maxRetries; attempt++)
            {
                try
                {
                    var response = await _httpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, ct);

                    response.EnsureSuccessStatusCode();

                    var contentType = response.Content.Headers.ContentType?.MediaType;

                    if (contentType != "application/pdf")
                    {
                        throw new InvalidOperationException($"Unsupported media type: {contentType}. Expected application/pdf.");
                    }

                    // Copy the stream to a MemoryStream to get ownership over the stream, to avoid it from being garbage collected or lost in an async flow 
                    var memoryStream = new MemoryStream();
                    await response.Content.CopyToAsync(memoryStream, ct);

                    memoryStream.Seek(0, SeekOrigin.Begin);

                    // PDF downloaded
                    return new DownloadResult
                    {
                        FileName = reports.FileName,
                        Uri = uri,
                        Success = true,
                        Stream = memoryStream
                    };
                }
                catch (OperationCanceledException)
                {
                    throw; // respect cancellation
                }
                catch (HttpRequestException ex) when (attempt < _maxRetries && IsTransient(ex))
                {
                    Console.WriteLine($"Attempt {attempt} failed for {uri}. Retrying in {_delay.TotalSeconds}s... Error: {ex.Message}");
                    await Task.Delay(TimeSpan.FromSeconds(Math.Pow(_delay.Seconds, attempt)), ct);
                }
                catch (Exception ex)
                {
                    // Log the exception for further investigation
                    Console.WriteLine($"Error downloading file: {ex.Message}");
                }
            }
        }

        // Download failed
        return new DownloadResult
        {
            FileName = reports.FileName,
            Uri = reports.PdfUri ?? reports.ReportHtmlUri,
            Success = false
        };
    }

    private bool IsTransient(HttpRequestException ex)
    {
        if (ex.StatusCode.HasValue)
        {
            int code = (int)ex.StatusCode.Value;
            // Retry only on 5xx server errors
            if (code >= 500 && code <= 599)
                return true;
        }

        // Check for network-level errors or timeout exceptions
        if (ex.InnerException is TimeoutException)
            return true;

        return false; // everything else (403, 404, 400) is permanent
    }
}
