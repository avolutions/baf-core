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
                
                // Ensure ultimate fallback
                if (settings.AvailableLanguages.Count == 0)
                {
                    settings.AvailableLanguages = ["en"];
                }
                
                if (settings.AvailableCultures.Count == 0)
                {
                    settings.AvailableCultures = ["en-US"];
                }
                
                if (string.IsNullOrWhiteSpace(settings.DefaultLanguage))
                {
                    settings.DefaultLanguage = "en";
                }
                
                if (string.IsNullOrWhiteSpace(settings.DefaultCulture))
                {
                    settings.DefaultCulture = "en-US";
                }
                
                // Initialize static BAF context
                LocalizationContext.Initialize(settings);
                
                var cultures = settings.AvailableCultures
                    .Select(c => new CultureInfo(c))
                    .ToList();

                options.SupportedCultures = cultures;
                options.SupportedUICultures = cultures;
                options.DefaultRequestCulture = new RequestCulture(settings.DefaultCulture);
                options.FallBackToParentCultures = true;
                options.FallBackToParentUICultures = true;
            });
    }
}