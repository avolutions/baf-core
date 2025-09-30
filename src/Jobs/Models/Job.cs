using Avolutions.Baf.Core.Jobs.Abstractions;

namespace Avolutions.Baf.Core.Jobs.Models;

public abstract class Job<TParam> : IJob
{
    public abstract string Key { get; }
    public abstract string Name { get; }
    public Type ParamType => typeof(TParam);
    public abstract Task<JobResult> ExecuteAsync(TParam param, CancellationToken ct);
    
    async Task<JobResult> IJob.ExecuteAsync(object p, CancellationToken ct)
        => await ExecuteAsync((TParam)p, ct);
}