using Avolutions.Baf.Core.Module.Abstractions;
using Avolutions.Baf.Core.Setup.Infrastructure;
using Avolutions.Baf.Core.Setup.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Avolutions.Baf.Core.Setup;

public class SetupModule : IFeatureModule
{
    public void Register(IServiceCollection services)
    {
        services.AddSingleton<SetupState>();
        services.AddScoped<ISetupService, SetupService>();
    }

    public async Task InitializeAsync(IServiceProvider services, CancellationToken ct = default)
    {
        var setup = services.GetRequiredService<ISetupService>();
        await setup.InitializeAsync(ct);
    }
}