
using Microsoft.Extensions.Options;

public class XlsxPathProvider : IXlsxPathProvider
{
    private readonly string _baseDirectory;

    public XlsxPathProvider(IOptions<AppSettings> settings)
    {
        _baseDirectory = Path.Combine(Directory.GetCurrentDirectory(), settings.Value.OutputFolder);
        // Ensure the Downloads directory exists
        Directory.CreateDirectory(_baseDirectory);
    }
    public string GetDestinationPath(string fileName) => Path.Combine(_baseDirectory, fileName + ".xlsx");
}