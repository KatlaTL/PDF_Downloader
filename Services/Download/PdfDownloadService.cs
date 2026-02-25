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

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Request failed with HTTP status code {response.StatusCode}. " +
                    $"The status code indicates an error, and it was outside the successful range (200-299).");
        }

        var contentType = response.Content.Headers.ContentType?.MediaType;

        if (contentType != "application/pdf")
        {
            throw new InvalidOperationException($"Unsupported media type: {contentType}. Expected application/pdf.");
        }

        return await response.Content.ReadAsStreamAsync(ct);
    }
}