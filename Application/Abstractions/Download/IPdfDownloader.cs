
public interface IPdfDownloader
{
    public Task<DownloadResult> DownloadAsync(Report reports, CancellationToken ct);
}