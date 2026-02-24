
public class PdfDownloaderService : IPdfDownloaderService
{
    private static readonly HttpClient _httpClient = new HttpClient();
    public async Task<string> DownloadFileAsync(Rapport pdf, CancellationToken ct)
    {
        var urlsToTry = new[] { pdf.PdfUrl, pdf.ReportHtmlUrl };
        foreach (var url in urlsToTry.Where(url => !string.IsNullOrWhiteSpace(url)))
        {
            if (url == null) continue;

            try
            {
                // If the file starts with file:// we try to copy it from disk, otherwise we will handle it as HTTP/HTTPS URLs
                if (url.StartsWith("file://", StringComparison.OrdinalIgnoreCase))
                {
                    string localFilePath = new Uri(url).LocalPath; // Remove file://

                    if (File.Exists(localFilePath))
                    {
                        File.Copy(localFilePath, GetDestinationPath(pdf.FileName), overwrite: true);
                    }
                    else
                    {
                        throw new FileNotFoundException($"Local file not found: {localFilePath}");
                    }
                }
                else
                {
                    using var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, ct);

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

                    await using var fs = new FileStream(GetDestinationPath(pdf.FileName), FileMode.Create, FileAccess.Write, FileShare.None);

                    await response.Content.CopyToAsync(fs, ct);
                }

                return url; // PDF downloaded
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

    private string GetDestinationPath(string fileName)
    {
        string downloadPath = Path.Combine(Directory.GetCurrentDirectory(), "Downloads");

        // Ensure the Downloads directory exists
        Directory.CreateDirectory(downloadPath);

        return Path.Combine(downloadPath, fileName + ".pdf");
    }
}