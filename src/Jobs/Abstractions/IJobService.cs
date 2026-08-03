using Avolutions.Baf.Core.Jobs.Models;

namespace Avolutions.Baf.Core.Jobs.Abstractions;

public interface IJobService
{
    Task<Guid> EnqueueAsync(string jobKey, object param, Guid? triggeredBy = null, CancellationToken ct = default);
    Task<IReadOnlyList<IJob>> GetAvailableJobsAsync();
    Task<List<JobRun>> GetRecentRunsAsync(int take = 100, string? jobKey = null);
    JobScheduleInfo? GetSchedule(string jobKey);
    IReadOnlyList<JobScheduleInfo> GetSchedules();
    bool IsManuallyTriggerable(string jobKey);
}