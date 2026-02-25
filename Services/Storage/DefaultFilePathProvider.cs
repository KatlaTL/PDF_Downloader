public class DefaultFilePathProvider : IFilePathProvider
{
    private readonly string _baseDirectory;

    public DefaultFilePathProvider()
    {
        _baseDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Downloads");
    }

    public string GetDestinationPathPdf(string fileName)
    {
        // Ensure the Downloads directory exists
        Directory.CreateDirectory(_baseDirectory);

        return Path.Combine(_baseDirectory, fileName + ".pdf");
    }
    public string GetDestinationPathXlsx(string fileName)
    {
        // Ensure the Downloads directory exists
        Directory.CreateDirectory(_baseDirectory);

        return Path.Combine(_baseDirectory, fileName + ".xlsx");
    }
}