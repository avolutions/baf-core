using Avolutions.Baf.Core.Entity.Abstractions;
using Avolutions.Baf.Core.Identity.Caching;
using Avolutions.Baf.Core.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Avolutions.Baf.Core.Identity.Services;

public class UserService : IEntityService<User>
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IUserCache _userCache;

    public UserService(UserManager<User> userManager, RoleManager<Role> roleManager, IUserCache userCache)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _userCache = userCache;
    }
    
    public async Task<List<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await GetAllAsync(false, cancellationToken);
    }
    
    public async Task<List<User>> GetAllAsync(bool includeSystemUser = false, CancellationToken cancellationToken = default)
    {
        var query = _userManager.Users.AsQueryable();

        if (!includeSystemUser)
        {
            query = query.Where(u => u.UserName != SystemUser.UserName);
        }

        var users = await query.ToListAsync(cancellationToken);

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            user.RoleName = roles.FirstOrDefault() ?? string.Empty; // Assumes one role per user
        }

        return users;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);

        if (user != null)
        {
            var roles = await _userManager.GetRolesAsync(user);
            user.RoleName = roles.FirstOrDefault() ?? string.Empty;
        }

        return user;
    }
    
    // Get by id or return system user as default
    public async Task<User> GetByIdOrDefaultAsync(Guid id)
    {
        var user = await GetByIdAsync(id);
        if (user is not null)
        {
            return user;
        }

        var systemUser = await GetByIdAsync(SystemUser.Id);
        if (systemUser is null)
        {
            throw new InvalidOperationException("System user not found in the database.");
        }

        return systemUser;
    }

    public Task<User> CreateAsync(User user, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<User> CreateAsync(User user, string password)
    {
        if (!await _roleManager.RoleExistsAsync(user.RoleName))
        {
            throw new Exception($"The role '{user.RoleName}' does not exist.");
        }

        user.AvatarColor = AvatarColors.GetRandom().Background;
        
        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            var errors = string.Join(Environment.NewLine, result.Errors.Select(e => e.Description));
            throw new Exception(errors);
        }

        await _userManager.AddToRoleAsync(user, user.RoleName);
        await _userCache.RefreshAsync();

        return user;
    }
    
    public async Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        if (!await _roleManager.RoleExistsAsync(user.RoleName))
        {
            throw new Exception($"The role '{user.RoleName}' does not exist.");
        }
        
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join(Environment.NewLine, result.Errors.Select(e => e.Description));
            throw new Exception(errors);
        }

        // Get current roles of the user
        var currentRoles = await _userManager.GetRolesAsync(user);
        
        // Remove from all existing roles
        var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
        if (!removeResult.Succeeded)
        {
            var errors = string.Join(", ", removeResult.Errors.Select(e => e.Description));
            throw new Exception(errors);
        }

        // Add to the new role
        var addResult = await _userManager.AddToRoleAsync(user, user.RoleName);
        if (!addResult.Succeeded)
        {
            var errors = string.Join(", ", addResult.Errors.Select(e => e.Description));
            throw new Exception(errors);
        }

        return user;
    }

    public Task DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<User?> GetByExternalIdAsync(string externalId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> ToggleLockoutAsync(User user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        if (user.IsLocked())
        {
            user.LockoutEnabled = false;
            user.LockoutEnd = null;
        }
        else
        {
            user.LockoutEnabled = true;
            user.LockoutEnd = DateTimeOffset.MaxValue;
        }

        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }
    
    public async Task ResetPasswordAsync(User user, string newPassword)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

        if (!result.Succeeded)
        {
            var errors = string.Join(Environment.NewLine, result.Errors.Select(e => e.Description));
            throw new InvalidOperationException(errors);
        }
    }
}