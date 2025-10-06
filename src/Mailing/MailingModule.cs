using Avolutions.Baf.Core.Mailing.Abstractions;
using Avolutions.Baf.Core.Mailing.Services;
using Avolutions.Baf.Core.Module.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Avolutions.Baf.Core.Mailing;

public class MailingModule : IFeatureModule
{
    public void Register(IServiceCollection services)
    {
        services.AddScoped<IMailService, MailService>();
    }
}