using Avolutions.Baf.Core.Entity.Abstractions;
using Avolutions.Baf.Core.Identity.Models;
using Avolutions.Baf.Core.Persistence.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Avolutions.Baf.Core.Entity.Configurations;

public class EntityConfiguration : IModelConfiguration
{
    public void Configure(ModelBuilder modelBuilder)
    {
        // Apply default values to all IEntity entities
        foreach (var entityType in modelBuilder.Model.GetEntityTypes()
                     .Where(t => typeof(IEntity).IsAssignableFrom(t.ClrType)))
        {
            var builder = modelBuilder.Entity(entityType.ClrType);

            builder.Property(nameof(IEntity.CreatedAt))
                .HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'");

            builder.Property(nameof(IEntity.ModifiedAt))
                .HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'");

            builder.Property(nameof(IEntity.CreatedBy))
                .HasDefaultValue(SystemUser.Id);

            builder.Property(nameof(IEntity.ModifiedBy))
                .HasDefaultValue(SystemUser.Id);

            builder
                .HasIndex(nameof(IEntity.ExternalId));
        }
    }
}