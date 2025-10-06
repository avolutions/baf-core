using Avolutions.Baf.Core.Settings.Attributes;
using Avolutions.Baf.Core.Settings.Models;

namespace Avolutions.Baf.Core.Mailing.Settings;

[Settings("Mail")]
public class MailSettings
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 587;
    public string User { get; set; } = string.Empty;
    public string From { get; set; } = string.Empty;
    public ProtectedSetting Password { get; set; } = null!;
}