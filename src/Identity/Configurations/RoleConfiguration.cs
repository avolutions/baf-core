using Avolutions.Baf.Core.Identity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Avolutions.Baf.Core.Identity.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");
        
        builder.HasData(
            new Role
            {
                Id = Guid.Parse("f16f5123-7897-41d6-8844-e00c1b4b390e"),
                Name = SystemRoles.Administrator,
                NormalizedName = SystemRoles.Administrator.ToUpper()
            },
            new Role
            {
                Id = Guid.Parse("580f2ddd-cfb8-4a7c-bd0e-052609b68dcc"),
                Name = SystemRoles.User,
                NormalizedName = SystemRoles.User.ToUpper()
            }
        );
    }
}