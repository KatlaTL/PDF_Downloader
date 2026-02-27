
public interface IPdfDownloadService
{
    public Task<DownloadResult> DownloadAsync(Report reports, CancellationToken ct);
}