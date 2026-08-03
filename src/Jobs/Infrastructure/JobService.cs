using System.Globalization;
using System.Text.Json;
using System.Threading.Channels;
using Avolutions.Baf.Core.Identity.Models;
using Avolutions.Baf.Core.Jobs.Abstractions;
using Avolutions.Baf.Core.Jobs.Models;
using Avolutions.Baf.Core.Persistence;
using CronExpressionDescriptor;
using Cronos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Avolutions.Baf.Core.Jobs.Infrastructure;

public sealed class JobService : IJobService
{    
    public const string SchedulesSection = "Jobs:Schedules";
    
    private readonly Channel<JobRequest> _channel;
    private readonly IJobRegistry _registry;
    private readonly BafDbContext _db;
    private readonly IConfiguration _configuration;

    public JobService(
        Channel<JobRequest> channel,
        IJobRegistry registry,
        BafDbContext db,
        IConfiguration configuration)
    {
        _channel = channel;
        _registry = registry;
        _db = db;
        _configuration = configuration;
    }

    public async Task<Guid> EnqueueAsync(string jobKey, object param, Guid? triggeredBy = null, CancellationToken ct = default)
    {
        var job = _registry.Get(jobKey) ?? throw new InvalidOperationException($"Unknown job '{jobKey}'.");
        var triggerUserId = triggeredBy ?? SystemUser.Id;

        if (triggerUserId != SystemUser.Id && !IsManuallyTriggerable(jobKey))
        {
            throw new InvalidOperationException($"Job '{jobKey}' cannot be triggered manually.");
        }

        var json = JsonSerializer.Serialize(param, job.ParamType);

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
    
    public JobScheduleInfo? GetSchedule(string jobKey)
    {
        if (_registry.Get(jobKey) is null)
        {
            return null;
        }

        var section = _configuration.GetSection($"{SchedulesSection}:{jobKey}");
        if (!section.Exists())
        {
            return null;
        }

        var options = section.Get<JobScheduleOptions>();
        if (options is null || string.IsNullOrWhiteSpace(options.Cron))
        {
            return null;
        }

        var timeZone = string.IsNullOrWhiteSpace(options.TimeZone)
            ? TimeZoneInfo.Utc
            : TimeZoneInfo.FindSystemTimeZoneById(options.TimeZone);

        var description = ExpressionDescriptor.GetDescription(options.Cron, new Options
        {
            Locale = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName,
            Use24HourTimeFormat = true
        });

        DateTimeOffset? nextRun = null;
        if (options.Enabled)
        {
            try
            {
                var format = options.Cron.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length > 5
                    ? CronFormat.IncludeSeconds
                    : CronFormat.Standard;

                nextRun = CronExpression.Parse(options.Cron, format)
                    .GetNextOccurrence(DateTimeOffset.UtcNow, timeZone);
            }
            catch
            {
                // invalid expression - already logged at startup
            }
        }

        return new JobScheduleInfo(jobKey, options.Cron, description, timeZone, options.Enabled, options.AllowManualTrigger, nextRun);
    }

    public IReadOnlyList<JobScheduleInfo> GetSchedules()
    {
        return _registry.All
            .Select(j => GetSchedule(j.Key))
            .Where(s => s is not null)
            .Select(s => s!)
            .ToList();
    }
    
    public bool IsManuallyTriggerable(string jobKey)
    {
        var schedule = GetSchedule(jobKey);

        return schedule is null || schedule.AllowManualTrigger;
    }
}
