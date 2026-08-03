namespace Avolutions.Baf.Core.Jobs.Models;

public sealed record JobScheduleInfo(
    string JobKey,
    string Cron,
    string Description,
    TimeZoneInfo TimeZone,
    bool Enabled,
    bool AllowManualTrigger,
    DateTimeOffset? NextRun);