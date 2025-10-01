using Avolutions.Baf.Core.Jobs.Models;

namespace Avolutions.Baf.Core.Jobs.Abstractions;

public interface IJob
{
    string Key { get; }
    string Name { get; }
    string Description { get; }
    Type ParamType { get; }
    Task<JobResult> ExecuteAsync(object parameters, CancellationToken ct);
}