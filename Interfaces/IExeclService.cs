
public interface IExcelService
{
    public List<Rapport> ReadLinks(string path, int? rows);

    public void ExportToExcel(List<DownloadResult> downloadResults);
}