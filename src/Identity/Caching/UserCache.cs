using System.Collections.Concurrent;
using Avolutions.Baf.Core.Identity.Abstractions;
using Avolutions.Baf.Core.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Avolutions.Baf.Core.Identity.Caching;

public class UserCache : IUserCache
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IUserDisplayService _displayService;
    private readonly ConcurrentDictionary<Guid, UserInfo> _cache = new();

    public UserCache(IServiceScopeFactory scopeFactory, IUserDisplayService displayService)
    {
        _scopeFactory = scopeFactory;
        _displayService = displayService;
    }

    public UserInfo Get(Guid userId)
    {
        return _cache.TryGetValue(userId, out var userInfo)
            ? userInfo
            : UserInfo.Unknown;
    }

    public async Task RefreshAsync(CancellationToken cancellationToken = default)
    {
        using var scope = _scopeFactory.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        var users = await userManager.Users
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        _cache.Clear();
        
        foreach (var user in users)
        {
            var userInfo = new UserInfo(
                user.Id,
                _displayService.GetName(user),
                _displayService.GetInitials(user),
                _displayService.GetAvatarColor(user)
            );

            _cache[user.Id] = userInfo;
        }
    }
}