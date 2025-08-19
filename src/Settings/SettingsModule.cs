using Avolutions.Baf.Core.Module.Abstractions;
using Avolutions.Baf.Core.Settings.Abstractions;
using Avolutions.Baf.Core.Settings.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Avolutions.Baf.Core.Settings;

public class SettingsModule : IFeatureModule
{
    public void Register(IServiceCollection services)
    {
        services.AddSingleton<ISettingsStore, SettingsStore>();
        services.AddSingleton(typeof(ISettings<>), typeof(SettingsAdapter<>));
    }

    public async Task InitializeAsync(IServiceProvider services, CancellationToken ct = default)
    {
        await services.GetRequiredService<ISettingsStore>().InitializeAsync(ct);
    }
}