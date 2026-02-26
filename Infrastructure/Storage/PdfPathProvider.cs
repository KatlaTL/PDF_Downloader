using Microsoft.Extensions.Options;

public class PdfPathProvider : IPdfPathProvider
{
    private readonly string _baseDirectory;

    public PdfPathProvider(IOptions<AppSettings> settings)
    {
        _baseDirectory = Path.Combine(Directory.GetCurrentDirectory(), settings.Value.OutputFolder);
        // Ensure the Downloads directory exists
        Directory.CreateDirectory(_baseDirectory);
    }

    public string GetDestinationPath(string fileName) => Path.Combine(_baseDirectory, fileName + ".pdf");
}