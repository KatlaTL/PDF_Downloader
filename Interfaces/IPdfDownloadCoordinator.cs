
public interface IPdfDownloadCoordinator
{
    public Task<Uri> DownloadAndSaveFileAsync(Rapport pdf, CancellationToken ct);
}