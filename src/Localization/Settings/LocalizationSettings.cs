using Avolutions.Baf.Core.Settings.Attributes;

namespace Avolutions.Baf.Core.Localization.Settings;

[Settings("Localization")]
public class LocalizationSettings
{
    public List<string> AvailableLanguages { get; set; } = [ "de", "en" ];
    public List<string> AvailableCultures { get; set; } = ["de-DE", "en-US"];
    public string DefaultLanguage { get; set; } = "en";
    public string DefaultCulture { get; set; } = "en-US";
}