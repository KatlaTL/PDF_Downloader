
public interface IFileDownloadService
{
    public Task<Stream> DownloadAsyncWithRetry(Uri uri, CancellationToken ct, int maxRetries = 3, TimeSpan? delay = null);
    public Task<Stream> DownloadAsync(Uri uri, CancellationToken ct);
}