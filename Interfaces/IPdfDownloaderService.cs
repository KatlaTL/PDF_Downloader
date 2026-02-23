
public interface IPdfDownloaderService
{
    public Task DownloadFileAsync(Rapport pdf, CancellationToken ct);
}