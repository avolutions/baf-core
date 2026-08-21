using System.Security.Claims;
using Avolutions.Baf.Core.Identity.Abstractions;
using Avolutions.Baf.Core.Identity.Caching;
using Avolutions.Baf.Core.Identity.Extensions;
using Avolutions.Baf.Core.Identity.Models;
using Microsoft.AspNetCore.Components.Authorization;

namespace Avolutions.Baf.Core.Identity.Services;

public class CurrentUser : ICurrentUser
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly IUserCache _userCache;

    public CurrentUser(
        AuthenticationStateProvider authenticationStateProvider,
        IUserCache userCache)
    {
        _authenticationStateProvider = authenticationStateProvider;
        _userCache = userCache;
    }

    public async Task<ClaimsPrincipal> GetPrincipalAsync()
    {
        var state = await _authenticationStateProvider.GetAuthenticationStateAsync();

        return state.User;
    }

    public async Task<Guid?> GetIdAsync()
    {
        var principal = await GetPrincipalAsync();

        return principal.GetUserId();
    }

    public async Task<Guid> GetRequiredIdAsync()
    {
        var id = await GetIdAsync();

        if (id is null)
        {
            throw new InvalidOperationException("No authenticated user in the current context.");
        }

        return id.Value;
    }

    public async Task<UserInfo> GetUserInfoAsync()
    {
        var id = await GetIdAsync();

        if (id is null)
        {
            return UserInfo.Unknown;
        }

        return _userCache.Get(id.Value);
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var principal = await GetPrincipalAsync();

        return principal.IsAuthenticated();
    }
}