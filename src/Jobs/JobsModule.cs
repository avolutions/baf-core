using System.Threading.Channels;
using Avolutions.Baf.Core.Jobs.Abstractions;
using Avolutions.Baf.Core.Jobs.Extensions;
using Avolutions.Baf.Core.Jobs.Infrastructure;
using Avolutions.Baf.Core.Jobs.Models;
using Avolutions.Baf.Core.Module.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NCronJob;

namespace Avolutions.Baf.Core.Jobs;

public class JobsModule : IFeatureModule
{
    public void Register(IServiceCollection services)
    {
        services.AddSingleton(Channel.CreateUnbounded<JobRequest>());
        services.AddSingleton<IJobRegistry, JobRegistry>();
        services.AddScoped<IJobService, JobService>();
        services.AddHostedService<JobWorker>();
        
        services.AddJobScheduling();
    }

    public void Configure(WebApplication app)
    {
        app.Lifetime.ApplicationStarted.Register(() => app.Services.UseJobScheduling());
        
        app.UseNCronJobAsync()
            .GetAwaiter()
            .GetResult();
    }
}