using Microsoft.Extensions.Options;
using Moq;

namespace PdfDownloader.Application.UseCases;


public class DownloadReportsUseCaseTests
{
    private readonly Mock<IReportSource> _reportSourceMock;
    private readonly Mock<IPdfDownloadService> _downloaderMock;
    private readonly Mock<IStorageService> _storageMock;
    private readonly Mock<IResultExporter> _exporterMock;
    private readonly DownloadReportsUseCase _useCase;

    public DownloadReportsUseCaseTests()
    {
        _reportSourceMock = new Mock<IReportSource>();
        _downloaderMock = new Mock<IPdfDownloadService>();
        _storageMock = new Mock<IStorageService>();
        _exporterMock = new Mock<IResultExporter>();

        var settings = Options.Create(new AppSettings
        {
            MaxConcurrency = 2,
            InputExcelPath = "GRI_2017_2020.xlsx",
            MaxRows = 10
        });

        _useCase = new DownloadReportsUseCase(
            _reportSourceMock.Object,
            _downloaderMock.Object,
            _storageMock.Object,
            _exporterMock.Object,
            settings);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Call_ReadLinks_With_Settings_Values()
    {
        _reportSourceMock
            .Setup(x => x.ReadLinks(It.IsAny<string>(), It.IsAny<int>()))
            .Returns(new List<Report>());

        await _useCase.ExecuteAsync(CancellationToken.None);

        _reportSourceMock.Verify(x => x.ReadLinks("GRI_2017_2020.xlsx", 10), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Call_DownloadAsync_For_Each_Report()
    {
        var testReports = new List<Report>
        {
            new (){ FileName = "r1", PdfUri = null},
            new (){ FileName = "r2", PdfUri = null},
            new (){ FileName = "r3", PdfUri = null},
        };

        _reportSourceMock
            .Setup(x => x.ReadLinks(It.IsAny<string>(), It.IsAny<int>()))
            .Returns(testReports);

        _downloaderMock
            .Setup(x => x.DownloadAsync(It.IsAny<Report>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Report report, CancellationToken ct) => new DownloadResult
            {
                FileName = report.FileName,
                Success = true,
                Stream = new MemoryStream(),
                Uri = null
            });

        await _useCase.ExecuteAsync(CancellationToken.None);

        foreach (var report in testReports)
        {
            _downloaderMock.Verify(x => x.DownloadAsync(report, It.IsAny<CancellationToken>()), Times.Once);
        }
    }

    [Fact]
    public async Task ExecuteAsync_Should_Save_Successful_Downloads()
    {
        _reportSourceMock
            .Setup(x => x.ReadLinks(It.IsAny<string>(), It.IsAny<int>()))
            .Returns(new List<Report>() { new() { FileName = "r1", PdfUri = null } });

        _downloaderMock
            .Setup(x => x.DownloadAsync(It.IsAny<Report>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Report report, CancellationToken ct) => new DownloadResult
            {
                FileName = report.FileName,
                Success = true,
                Stream = new MemoryStream(),
                Uri = null
            });

        await _useCase.ExecuteAsync(CancellationToken.None);

        _storageMock
            .Verify(x => x.SaveAsync("r1", It.Is<MemoryStream>(s => s.Length == 0), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Not_Save_Unsuccessful_Downloads()
    {
        _reportSourceMock
            .Setup(x => x.ReadLinks(It.IsAny<string>(), It.IsAny<int>()))
            .Returns(new List<Report>() { new() { FileName = "r1", PdfUri = null } });

        _downloaderMock
            .Setup(x => x.DownloadAsync(It.IsAny<Report>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Report report, CancellationToken ct) => new DownloadResult
            {
                FileName = report.FileName,
                Success = false,
                Stream = new MemoryStream(),
                Uri = null
            });

        await _useCase.ExecuteAsync(CancellationToken.None);

        _storageMock
            .Verify(x => x.SaveAsync("r1", It.Is<MemoryStream>(s => s.Length == 0), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Not_Save_When_Stream_Is_Null()
    {
        _reportSourceMock
           .Setup(x => x.ReadLinks(It.IsAny<string>(), It.IsAny<int>()))
           .Returns(new List<Report>() { new() { FileName = "r1", PdfUri = null } });

        _downloaderMock
            .Setup(x => x.DownloadAsync(It.IsAny<Report>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Report report, CancellationToken ct) => new DownloadResult
            {
                FileName = report.FileName,
                Success = true,
                Stream = null,
                Uri = null
            });

        await _useCase.ExecuteAsync(CancellationToken.None);

        _storageMock
            .Verify(x => x.SaveAsync("r1", It.Is<MemoryStream>(s => s.Length == 0), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Call_Exporter_With_Results()
    {
        var testReports = new List<Report>
        {
            new (){ FileName = "r1", PdfUri = null},
            new (){ FileName = "r2", PdfUri = null},
            new (){ FileName = "r3", PdfUri = null},
        };

        _reportSourceMock
            .Setup(x => x.ReadLinks(It.IsAny<string>(), It.IsAny<int>()))
            .Returns(testReports);

        _downloaderMock
            .Setup(x => x.DownloadAsync(It.IsAny<Report>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Report report, CancellationToken ct) => new DownloadResult
            {
                FileName = report.FileName,
                Success = true,
                Stream = new MemoryStream(),
                Uri = null
            });

        _storageMock
            .Setup(x => x.SaveAsync(It.IsAny<string>(), It.IsAny<MemoryStream>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await _useCase.ExecuteAsync(CancellationToken.None);

        _exporterMock
            .Verify(x => x.ExportAsync(It.Is<List<DownloadResult>>(r => r.Count == 3)), Times.Once);
    }
}