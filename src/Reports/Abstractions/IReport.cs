namespace Avolutions.Baf.Core.Reports.Abstractions;

public interface IReport
{
    string ContentTemplatePath { get; }
    string? HeaderTemplatePath { get; }
    string? FooterTemplatePath { get; }
    Task<IReportModel> BuildModelAsync(IReportArgs args, CancellationToken ct = default);
}