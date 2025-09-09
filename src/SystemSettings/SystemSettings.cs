using Avolutions.Baf.Core.Settings.Attributes;

namespace Avolutions.Baf.Core.SystemSettings;

[Settings("System")]
public class SystemSettings
{
    public string ApplicationTitle { get; set; } = "BAF App";
}