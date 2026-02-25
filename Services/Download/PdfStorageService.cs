public class PdfStorageService : IStorageService
{
    public async Task SaveAsync(string fileName, Stream content, CancellationToken ct)
    {
        // Ensure the stream is read from the beginning
        content.Seek(0, SeekOrigin.Begin);

        using var fs = new FileStream(GetDestinationPath(fileName), FileMode.Create, FileAccess.Write, FileShare.None);

        await content.CopyToAsync(fs, ct);
    }

    private string GetDestinationPath(string fileName)
    {
        string downloadPath = Path.Combine(Directory.GetCurrentDirectory(), "Downloads");

        // Ensure the Downloads directory exists
        Directory.CreateDirectory(downloadPath);

        return Path.Combine(downloadPath, fileName + ".pdf");
    }
}