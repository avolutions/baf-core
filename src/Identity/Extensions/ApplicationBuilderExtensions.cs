using Avolutions.BAF.Core.Identity.Extensions;
using Microsoft.AspNetCore.Builder;

namespace Avolutions.Baf.Core.Identity.Extensions;

public static class ApplicationBuilderExtensions
{
    public static WebApplication UseBafIdentity(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
        
        app.MapAdditionalIdentityEndpoints();

        return app;
    }
}