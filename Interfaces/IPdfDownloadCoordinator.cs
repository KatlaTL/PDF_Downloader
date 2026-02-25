
public interface IPdfDownloadCoordinator
{
    public Task<List<DownloadResult>> DownloadAndSaveFilesAsync(List<Rapport> rapports, CancellationToken ct, int maxConcurrency = 10);
    public Task<DownloadResult> DownloadAndSaveFileAsync(Rapport rapport, CancellationToken ct);
}