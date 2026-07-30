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
                LocalizationContext.Initialize(localizationSettings.Value);

                var cultures = LocalizationContext.AvailableCultures
                    .Select(culture => new CultureInfo(culture))
                    .ToList();

                options.SupportedCultures = cultures;
                options.SupportedUICultures = cultures;
                options.DefaultRequestCulture = new RequestCulture(LocalizationContext.DefaultCulture);
                options.FallBackToParentCultures = true;
                options.FallBackToParentUICultures = true;
            });
    }
}