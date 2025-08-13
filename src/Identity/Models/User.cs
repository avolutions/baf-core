using Avolutions.BAF.Core.Entities.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace Avolutions.BAF.Core.Identity.Models;

public class User : IdentityUser<Guid>, IEntity
{
    public string? ExternalId { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime ModifiedAt { get; set; }
    public Guid ModifiedBy { get; set; }
}