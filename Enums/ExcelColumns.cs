public enum ExcelColumns
{
    [ColumnLetter("A")] FileName,
    [ColumnLetter("AL")] PdfUrl,
    [ColumnLetter("AM")] ReportHtmlUrl,
  
}

[AttributeUsage(AttributeTargets.Field)]
public class ColumnLetterAttribute : Attribute
{
    public string Letter {get;}
    public ColumnLetterAttribute(string letter) => Letter = letter; 
}