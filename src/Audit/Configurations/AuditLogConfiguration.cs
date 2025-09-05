using Avolutions.Baf.Core.Audit.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Avolutions.Baf.Core.Audit.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.Property(x => x.Entity)
            .HasMaxLength(200);
        builder.Property(x => x.Action)
            .HasConversion<string>();

        builder.Property(x => x.OldState)
            .HasColumnType("jsonb");

        builder.Property(x => x.NewState)
            .HasColumnType("jsonb");
        
        builder.HasIndex(x => new { x.Entity, x.EntityId, x.CreatedAt });
    }
}