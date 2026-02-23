
public interface IPdfDownloaderService
{
    public Task<string> DownloadFileAsync(Rapport pdf, CancellationToken ct);
}