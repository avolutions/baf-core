using Avolutions.Baf.Core.Common;
using Avolutions.BAF.Core.Entity.Abstractions;
using Avolutions.BAF.Core.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Avolutions.Baf.Core.Identity.Services;

public class UserService : IEntityService<User>
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;

    public UserService(UserManager<User> userManager, RoleManager<Role> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }
    
    public async Task<List<User>> GetAllAsync()
    {
        var users = await _userManager.Users.ToListAsync();

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

    public Task<User> CreateAsync(User user)
    {
        throw new NotImplementedException();
    }

    public async Task<User> CreateAsync(User user, string password)
    {
        if (!await _roleManager.RoleExistsAsync(user.RoleName))
        {
            throw new Exception($"The role '{user.RoleName}' does not exist.");
        }

        user.AvatarColor = MaterialColors.GetRandomColor().Background;
        
        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            var errors = string.Join(Environment.NewLine, result.Errors.Select(e => e.Description));
            throw new Exception(errors);
        }

        await _userManager.AddToRoleAsync(user, user.RoleName);

        return user;
    }
    
    public async Task<User> UpdateAsync(User user)
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

    public Task<Guid?> GetPreviousIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Guid?> GetNextIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<User?> GetByExternalIdAsync(string externalId)
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