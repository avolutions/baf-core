using System.ComponentModel.DataAnnotations.Schema;
using Avolutions.Baf.Core.Colors.Models;
using Avolutions.Baf.Core.Entity.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace Avolutions.Baf.Core.Identity.Models;

public class User : IdentityUser<Guid>, IEntity, ITrackable
{
    public string Firstname { get; set; } = string.Empty;
    public string Lastname { get; set; } = string.Empty;
    public string AvatarColor { get; set; } = MaterialColors.Grey;
    public string? ExternalId { get; set; }
    [NotMapped]
    public string RoleName { get; set; } = string.Empty;
    public bool IsLocked()
    {
        return LockoutEnabled && LockoutEnd > DateTimeOffset.UtcNow;
    }

    public string GetName()
    {
        return $"{Firstname} {Lastname}".Trim(); // TODO get from helper
    }

    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime ModifiedAt { get; set; }
    public Guid ModifiedBy { get; set; }
}