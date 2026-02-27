public class AppSettings
{
    public int MaxConcurrency { get; set; } = 10;
    public int MaxRows { get; set; } = 10;
    public string InputExcelPath { get; set; } = string.Empty;
    public string OutputFolder { get; set; } = string.Empty;
    public ExcelColumnSettings ExcelColumns {get; set;} = new();
}