
public class DownloadResult
{
    public Uri? Uri { get; set; }
    public string FileName { get; set; } = string.Empty;
    public bool Success { get; set; }

    // temporary stream for storage
    public Stream? Stream { get; init; }
    public string Status => Success ? "✅ Downloadet" : "❌ Ikke downloadet";
}