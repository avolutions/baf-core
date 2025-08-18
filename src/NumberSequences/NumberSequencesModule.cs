using Avolutions.BAF.Core.Module.Abstractions;
using Avolutions.BAF.Core.NumberSequences.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Avolutions.BAF.Core.NumberSequences;

public class NumberSequencesModule : IFeatureModule
{
    public void Register(IServiceCollection services)
    {
        services.AddScoped<INumberSequenceService, NumberSequenceService>();
        services.AddScoped(typeof(INumberSequenceService<>), typeof(NumberSequenceService<>));
    }
}