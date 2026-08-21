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
    
    /// <summary>
    /// Returns true when the current user satisfies the given policy.
    /// Use to show or hide UI and to filter collections.
    /// </summary>
    Task<bool> IsAuthorizedAsync(string policy);
    
    /// <summary>
    /// Throws <see cref="ForbiddenException"/> when the current user does not
    /// satisfy the given policy. Use in services where proceeding is never valid.
    /// </summary>
    Task DemandAsync(string policy);
}