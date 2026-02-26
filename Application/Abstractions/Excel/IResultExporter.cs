public interface IResultExporter
{
    public Task ExportAsync(IEnumerable<DownloadResult> results);
}