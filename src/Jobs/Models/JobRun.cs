using Avolutions.Baf.Core.Entity.Models;

namespace Avolutions.Baf.Core.Jobs.Models;

public class JobRun : EntityBase
{
    public string JobKey { get; set; } = default!;
    public string? TriggeredBy { get; set; }
    public DateTimeOffset QueuedAt { get; set; }
    public DateTimeOffset? StartedAt { get; set; }
    public DateTimeOffset? FinishedAt { get; set; }
    public JobRunStatus Status { get; set; }
    public string ParamJson { get; set; } = default!;
    public string? ResultJson { get; set; }
    public string? Error { get; set; }
}