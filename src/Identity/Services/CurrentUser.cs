using System.Security.Claims;
using Avolutions.Baf.Core.Identity.Abstractions;
using Avolutions.Baf.Core.Identity.Caching;
using Avolutions.Baf.Core.Identity.Exceptions;
using Avolutions.Baf.Core.Identity.Extensions;
using Avolutions.Baf.Core.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;

namespace Avolutions.Baf.Core.Identity.Services;

public class CurrentUser : ICurrentUser
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly IUserCache _userCache;
    private readonly IAuthorizationService _authorizationService;

    public CurrentUser(
        AuthenticationStateProvider authenticationStateProvider,
        IUserCache userCache,
        IAuthorizationService authorizationService)
    {
        _authenticationStateProvider = authenticationStateProvider;
        _userCache = userCache;
        _authorizationService = authorizationService;
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

    public async Task<bool> IsAuthorizedAsync(string policy)
    {
        var principal = await GetPrincipalAsync();
        var result = await _authorizationService.AuthorizeAsync(principal, policy);

        return result.Succeeded;
    }

    public async Task DemandAsync(string policy)
    {
        if (await IsAuthorizedAsync(policy))
        {
            return;
        }

        throw new ForbiddenException(policy);
    }
}