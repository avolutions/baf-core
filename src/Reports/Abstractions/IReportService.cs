namespace Avolutions.Baf.Core.Reports.Abstractions;

public interface IReportService
{
    Task<byte[]> RenderPdfAsync<TReport>(IReportArgs args, CancellationToken ct = default)
        where TReport : class, IReport;
}