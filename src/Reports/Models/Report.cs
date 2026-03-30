using Avolutions.Baf.Core.Reports.Abstractions;

namespace Avolutions.Baf.Core.Reports.Models;

public abstract class Report<TModel, TArgs> : IReport
    where TModel : IReportModel
    where TArgs : IReportArgs
{
    private string? _basePath;

    private string BasePath => _basePath ??= GetBasePath();

    private string GetBasePath()
    {
        var ns = GetType().Namespace?.Replace(".", "/") ?? "";
        var index = ns.IndexOf('/');
        if (index > 0)
        {
            ns = ns[(index + 1)..];
        }
    
        return Path.Combine(AppContext.BaseDirectory, ns, "Templates");
    }

    protected string TemplatePath(string fileName) => Path.Combine(BasePath, fileName);
    
    public abstract string ContentTemplatePath { get; }
    public virtual string? HeaderTemplatePath => null;
    public virtual string? FooterTemplatePath => null;

    public abstract Task<TModel> BuildModelAsync(TArgs args, CancellationToken ct = default);

    async Task<IReportModel> IReport.BuildModelAsync(IReportArgs args, CancellationToken ct)
        => await BuildModelAsync((TArgs)args, ct);
}