public class PdfDownloadService : IFileDownloadService
{
    private readonly HttpClient _httpClient;

    public PdfDownloadService(HttpClient httpClient)
    {
        _httpClient = httpClient;
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
}