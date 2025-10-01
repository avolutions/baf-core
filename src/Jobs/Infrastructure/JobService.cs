using System.Text.Json;
using System.Threading.Channels;
using Avolutions.Baf.Core.Identity.Models;
using Avolutions.Baf.Core.Jobs.Abstractions;
using Avolutions.Baf.Core.Jobs.Models;
using Microsoft.EntityFrameworkCore;

namespace Avolutions.Baf.Core.Jobs.Infrastructure;

public sealed class JobService : IJobService
{
    private readonly Channel<JobRequest> _channel;
    private readonly IJobRegistry _registry;
    private readonly BafDbContext _db;

    public JobService(Channel<JobRequest> channel, IJobRegistry registry, BafDbContext db)
    {
        _channel = channel;
        _registry = registry;
        _db = db;
    }

    public async Task<Guid> EnqueueAsync<TParam>(string jobKey, TParam param, Guid? triggeredBy = null, CancellationToken ct = default)
    {
        var job = _registry.Get(jobKey) ?? throw new InvalidOperationException($"Unknown job '{jobKey}'.");
        var json = JsonSerializer.Serialize(param, job.ParamType);
        var triggerUserId = triggeredBy ?? SystemUser.Id;
        
        var run = new JobRun
        {
            Id = Guid.NewGuid(),
            JobKey = jobKey,
            TriggeredBy = triggerUserId,
            QueuedAt = DateTimeOffset.UtcNow,
            Status = JobRunStatus.Queued,
            ParamJson = json
        };
        _db.JobRuns.Add(run);
        await _db.SaveChangesAsync(ct);

        await _channel.Writer.WriteAsync(new JobRequest(run.Id, jobKey, json), ct);
        return run.Id;
    }

    public Task<IReadOnlyList<IJob>> GetAvailableJobsAsync()
    {
        return Task.FromResult<IReadOnlyList<IJob>>(_registry.All);
    }

    public Task<List<JobRun>> GetRecentRunsAsync(int take = 100, string? jobKey = null)
    {
        return _db.JobRuns
            .AsNoTracking()
            .Where(r => jobKey == null || r.JobKey == jobKey)
            .OrderByDescending(r => r.QueuedAt)
            .Take(take)
            .ToListAsync();
    }
}
