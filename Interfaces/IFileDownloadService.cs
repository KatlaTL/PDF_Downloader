
public interface IFileDownloadService
{
    public Task<Stream> DownloadAsync(Uri uri, CancellationToken ct);
}