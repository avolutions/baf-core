using System.ComponentModel.DataAnnotations.Schema;
using Avolutions.Baf.Core.Entity.Models;
using Avolutions.Baf.Core.Reports.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Avolutions.Baf.Core.Reports.Models;

public abstract class Report : EntityBase
{
    public string Key { get; set; } = string.Empty;
    public string Json { get; set; } = string.Empty;
    public string HeaderHtml { get; set; } = string.Empty;
    public string ContentHtml { get; set; } = string.Empty;
    public string FooterHtml { get; set; } = string.Empty;

    // For tooling / reflection
    public abstract Type ModelType { get; }
    public abstract Type ArgsType { get; }

    public abstract Task<IReportModel> BuildModelAsync(
        DbContext db, IReportArgs? args, CancellationToken ct);

    public abstract Task<IReportModel> BuildDemoAsync(
        DbContext db, CancellationToken ct);
}

public abstract class Report<TModel, TArgs> : Report
    where TModel : class, IReportModel, new()
    where TArgs  : class, IReportArgs, new()
{
    [NotMapped]
    public TModel? Model { get; private set; }

    public sealed override Type ModelType => typeof(TModel);
    public sealed override Type ArgsType  => typeof(TArgs);

    protected abstract Task<TModel> BuildTypedModelAsync(
        DbContext db, TArgs args, CancellationToken ct);

    protected abstract Task<TModel> BuildTypedDemoAsync(
        DbContext db, CancellationToken ct);

    public sealed override async Task<IReportModel> BuildModelAsync(
        DbContext db, IReportArgs? args, CancellationToken ct)
    {
        if (args is not TArgs typed)
        {
            throw new ArgumentException(
                $"Report args must be of type {typeof(TArgs).Name}. " +
                $"Received {args?.GetType().Name ?? "null"}.", nameof(args));
        }

        return await BuildTypedModelAsync(db, typed, ct).ContinueWith<IReportModel>(t => t.Result, ct);
    }

    public sealed override async Task<IReportModel> BuildDemoAsync(
        DbContext db, CancellationToken ct)
    {
        return await BuildTypedDemoAsync(db, ct);
    }
}