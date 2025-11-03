namespace Avolutions.Baf.Core.Reports.Abstractions;

public interface IReportWithKey
{
    static abstract string ReportKey { get; }
}