public interface IFilePathProvider
{
    public string GetDestinationPathPdf(string fileName);

    public string GetDestinationPathXlsx(string fileName);
}