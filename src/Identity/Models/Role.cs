using Microsoft.AspNetCore.Identity;

namespace Avolutions.BAF.Core.Identity.Models;

/// <summary>
/// Role entity for BAF using Guid as the primary key.
/// </summary>
public class Role : IdentityRole<Guid>
{
    
}