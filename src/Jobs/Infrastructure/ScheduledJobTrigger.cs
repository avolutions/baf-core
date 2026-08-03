using Avolutions.Baf.Core.Identity.Models;
using Avolutions.Baf.Core.Jobs.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NCronJob;

namespace Avolutions.Baf.Core.Jobs.Infrastructure;

public sealed class ScheduledJobTrigger : NCronJob.IJob
{
    private readonly IJobService _jobService;
    private readonly IJobRegistry _registry;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ScheduledJobTrigger> _logger;

    public ScheduledJobTrigger(
        IJobService jobService,
        IJobRegistry registry,
        IConfiguration configuration,
        ILogger<ScheduledJobTrigger> logger)
    {
        _jobService = jobService;
        _registry = registry;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task RunAsync(IJobExecutionContext context, CancellationToken token)
    {
        if (context.Parameter is not string jobKey)
        {
            _logger.LogWarning("Scheduled trigger fired without a job key.");
            return;
        }

        var job = _registry.Get(jobKey);
        if (job is null)
        {
            _logger.LogWarning("Scheduled job '{JobKey}' is not registered.", jobKey);
            return;
        }

        var section = _configuration.GetSection($"Jobs:Schedules:{jobKey}:Param");
        var param = section.Exists()
            ? section.Get(job.ParamType)
            : Activator.CreateInstance(job.ParamType);

        await _jobService.EnqueueAsync(jobKey, param!, SystemUser.Id, token);
    }
}