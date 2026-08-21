using System.Security.Claims;
using Avolutions.Baf.Core.Identity.Models;

namespace Avolutions.Baf.Core.Identity.Abstractions;

public interface ICurrentUser
{
    Task<ClaimsPrincipal> GetPrincipalAsync();

    Task<Guid?> GetIdAsync();

    Task<Guid> GetRequiredIdAsync();

    Task<UserInfo> GetUserInfoAsync();

    Task<bool> IsAuthenticatedAsync();
}