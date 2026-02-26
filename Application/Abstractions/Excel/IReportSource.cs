
public interface IReportSource
{
    public IReadOnlyCollection<Report> ReadLinks(string path, int? rows = null);
}