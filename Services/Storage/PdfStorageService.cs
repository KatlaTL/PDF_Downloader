public class PdfStorageService : IStorageService
{
    private readonly IFilePathProvider _PathProvider;
    public PdfStorageService(IFilePathProvider PathProvider)
    {
        _PathProvider = PathProvider;
    }
    public async Task SaveAsync(string fileName, Stream content, CancellationToken ct)
    {
        // Ensure the stream is read from the beginning
        content.Seek(0, SeekOrigin.Begin);

        using var fs = new FileStream(_PathProvider.GetDestinationPathPdf(fileName), FileMode.Create, FileAccess.Write, FileShare.None);

        await content.CopyToAsync(fs, ct);
    }
}