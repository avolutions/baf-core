using Avolutions.Baf.Core.Localization.Settings;
using Avolutions.Baf.Core.Module.Abstractions;
using Avolutions.Baf.Core.Settings.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Avolutions.Baf.Core.Localization;

public class LocalizationModule : IFeatureModule
{
    public void Register(IServiceCollection services)
    {
        services.AddLocalization(options => options.ResourcesPath = "");
        
        services.AddOptions<RequestLocalizationOptions>()
            .Configure<ISettings<LocalizationSettings>>((options, localizationSettings) =>
            {
                var cultures = localizationSettings.Value.AvailableLanguages
                    .Select(culture => new System.Globalization.CultureInfo(culture)).ToList();

                options.SupportedCultures    = cultures;
                options.SupportedUICultures  = cultures;

                var fallback = string.IsNullOrWhiteSpace(localizationSettings.Value.DefaultLanguage)
                    ? "en"
                    : localizationSettings.Value.DefaultLanguage.ToLowerInvariant();

                options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture(fallback);
                options.FallBackToParentCultures = true;
                options.FallBackToParentUICultures = true;
            });
    }
}