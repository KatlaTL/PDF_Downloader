
public class Rapport
{
    public string FileName {get; set;} = string.Empty;
    public required Uri PdfUri {get; set;}
    public Uri? ReportHtmlUri {get; set;}
}