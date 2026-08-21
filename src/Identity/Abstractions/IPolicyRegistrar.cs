using Microsoft.AspNetCore.Authorization;

namespace Avolutions.Baf.Core.Identity.Abstractions;

public interface IPolicyRegistrar
{
    void Register(AuthorizationOptions options);
}