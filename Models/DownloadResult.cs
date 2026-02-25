
public class DownloadResult
{
    public Uri? Uri {get; set;}
    public string FileName {get; set;} = string.Empty;
    public bool Success {get; set;}

    public string Status => Success ? "✅ Downloadet" : "❌ Ikke downloadet";
}