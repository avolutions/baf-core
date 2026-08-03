namespace Avolutions.Baf.Core.Jobs.Models;

public sealed class JobScheduleOptions
{
    public string Cron { get; set; } = string.Empty;
    public string? TimeZone { get; set; }
    public bool Enabled { get; set; } = true;
    public bool AllowManualTrigger { get; set; }
}