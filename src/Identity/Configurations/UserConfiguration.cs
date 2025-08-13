using Avolutions.BAF.Core.Identity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Avolutions.BAF.Core.Identity.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        
        builder.HasData(new User
        {
            Id = SystemUser.Id,
            UserName = SystemUser.UserName,
            NormalizedUserName = SystemUser.NormalizedUserName,
            Email = SystemUser.Email,
            NormalizedEmail = SystemUser.NormalizedEmail,
            EmailConfirmed = true,
            SecurityStamp = SystemUser.SecurityStamp,
            ConcurrencyStamp = SystemUser.ConcurrencyStamp,
            LockoutEnabled = true,
            LockoutEnd = DateTimeOffset.MaxValue
        });
    }
}