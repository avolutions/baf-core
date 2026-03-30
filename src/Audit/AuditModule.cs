using Avolutions.Baf.Core.Audit.Abstractions;
using Avolutions.Baf.Core.Audit.Interceptors;
using Avolutions.Baf.Core.Audit.Services;
using Avolutions.Baf.Core.Module.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Avolutions.Baf.Core.Audit;

public class AuditModule : IFeatureModule
{
    public void Register(IServiceCollection services)
    {
        services.AddScoped(typeof(IAuditLogService<>), typeof(AuditLogService<>));
        services.TryAddSingleton<AuditSaveChangesInterceptor>();
    }
}