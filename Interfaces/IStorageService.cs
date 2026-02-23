public interface IStorageService
{
    public Task SaveAsync(string fileName, Stream content, CancellationToken ct);
}