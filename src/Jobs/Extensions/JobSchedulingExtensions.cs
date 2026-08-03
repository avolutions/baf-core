using Avolutions.Baf.Core.Jobs.Abstractions;
using Avolutions.Baf.Core.Jobs.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NCronJob;

namespace Avolutions.Baf.Core.Jobs.Extensions;

public static class JobSchedulingExtensions
{
    public const string SchedulesSection = "Jobs:Schedules";

    /// <summary>
    /// Registers the NCronJob infrastructure and the trigger job. Schedules themselves
    /// are added later in <see cref="UseJobScheduling"/>, once configuration is available.
    /// </summary>
    public static IServiceCollection AddJobScheduling(this IServiceCollection services)
    {
        services.AddTransient<ScheduledJobTrigger>();
        services.AddNCronJob();

        return services;
    }

    /// <summary>
    /// Reads the configured schedules and registers a cron trigger for each of them.
    /// </summary>
    public static void UseJobScheduling(this IServiceProvider serviceProvider)
    {
        var runtimeRegistry = serviceProvider.GetRequiredService<IRuntimeJobRegistry>();
        var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("JobScheduling");

        using var scope = serviceProvider.CreateScope();
        var jobService = scope.ServiceProvider.GetRequiredService<IJobService>();

        foreach (var schedule in jobService.GetSchedules())
        {
            if (!schedule.Enabled)
            {
                logger.LogInformation("Schedule for job '{JobKey}' is disabled.", schedule.JobKey);
                continue;
            }

            var succeeded = runtimeRegistry.TryRegister(
                n => n.AddJob<ScheduledJobTrigger>(p => p
                    .WithCronExpression(schedule.Cron, timeZoneInfo: schedule.TimeZone)
                    .WithParameter(schedule.JobKey)
                    .WithName(ScheduleName(schedule.JobKey))),
                out var exception);

            if (!succeeded)
            {
                logger.LogError(
                    exception,
                    "Could not schedule job '{JobKey}' with cron '{Cron}'.",
                    schedule.JobKey, schedule.Cron);
                continue;
            }

            logger.LogInformation(
                "Scheduled job '{JobKey}': {Description} ({TimeZone}).",
                schedule.JobKey, schedule.Description, schedule.TimeZone.Id);
        }
    }

    /// <summary>
    /// The NCronJob job name used for a scheduled BAF job.
    /// Use this to look up or mutate a schedule via IRuntimeJobRegistry.
    /// </summary>
    public static string ScheduleName(string jobKey) => $"schedule:{jobKey}";
}