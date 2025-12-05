using System.Globalization;
using Avolutions.Baf.Core.Localization.Settings;
using Avolutions.Baf.Core.Module.Abstractions;
using Avolutions.Baf.Core.Settings.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
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
                var settings = localizationSettings.Value;
                
                // Ensure ultimate language fallback
                if (settings.AvailableLanguages.Count == 0)
                {
                    settings.AvailableLanguages = ["en"];
                }
                
                if (string.IsNullOrWhiteSpace(settings.DefaultLanguage))
                {
                    settings.DefaultLanguage = "en";
                }
                
                // Initialize static BAF context
                LocalizationContext.Initialize(settings);
                
                var cultures = settings.AvailableLanguages
                    .Select(culture => new CultureInfo(culture)).ToList();

                options.SupportedCultures = cultures;
                options.SupportedUICultures = cultures;
                options.DefaultRequestCulture = new RequestCulture(settings.DefaultLanguage);
                options.FallBackToParentCultures = true;
                options.FallBackToParentUICultures = true;
            });
    }
}