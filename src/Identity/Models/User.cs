using System.ComponentModel.DataAnnotations.Schema;
using Avolutions.BAF.Core.Entities.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace Avolutions.BAF.Core.Identity.Models;

public class User : IdentityUser<Guid>, IEntity
{
    public string Firstname { get; set; } = string.Empty;
    public string Lastname { get; set; } = string.Empty;
    public string AvatarColor { get; set; } = string.Empty;
    public string? ExternalId { get; set; }
    [NotMapped]
    public string RoleName { get; set; } = string.Empty;
    public bool IsLocked()
    {
        return LockoutEnabled && LockoutEnd > DateTimeOffset.UtcNow;
    }
    public string GetName()
    {
        return $"{Firstname} {Lastname}";
    }
    public string GetInitials()
    {
        if (!string.IsNullOrEmpty(Firstname) && !string.IsNullOrEmpty(Lastname))
        {
            return $"{Firstname[0]}{Lastname[0]}".ToUpper();
        }
        return "";
    }
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime ModifiedAt { get; set; }
    public Guid ModifiedBy { get; set; }
}