using Avolutions.Baf.Core.Module.Abstractions;
using Avolutions.Baf.Core.NumberSequences.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Avolutions.Baf.Core.NumberSequences;

public class NumberSequencesModule : IFeatureModule
{
    public void Register(IServiceCollection services)
    {
        services.AddScoped<INumberSequenceService, NumberSequenceService>();
        services.AddScoped(typeof(INumberSequenceService<>), typeof(NumberSequenceService<>));
    }
}