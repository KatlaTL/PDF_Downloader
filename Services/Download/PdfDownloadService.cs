public class PdfDownloadService : IFileDownloadService
{
    private readonly HttpClient _httpClient;

    public PdfDownloadService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Stream> DownloadAsyncWithRetry(Uri uri, CancellationToken ct, int maxRetries = 3, TimeSpan? delay = null)
    {
        delay ??= TimeSpan.FromSeconds(2);

        for (int attempt = 1; attempt < maxRetries; attempt++)
        {
            try
            {
                return await DownloadAsync(uri, ct);
            }
            catch (HttpRequestException ex) when (attempt < maxRetries && IsTransient(ex))
            {
                Console.WriteLine($"Attempt {attempt} failed for {uri}. Retrying in {delay.Value.TotalSeconds}s... Error: {ex.Message}");
                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(delay.Value.Seconds, attempt)), ct);
            }
        }

        // If all retries failed
        throw new HttpRequestException($"Failed to download {uri} after {maxRetries} attempts");
    }

    public async Task<Stream> DownloadAsync(Uri uri, CancellationToken ct)
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

        return memoryStream;
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