using Avolutions.Baf.Core.Lookups.Abstractions;
using Avolutions.Baf.Core.Persistence.Abstractions;
using Humanizer;
using Microsoft.EntityFrameworkCore;

namespace Avolutions.Baf.Core.Lookups.Configurations;

public class LookupTranslationConfiguration : IModelConfiguration
{
    public void Configure(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes()
                     .Where(t => typeof(ILookupTranslation).IsAssignableFrom(t.ClrType)))
        {
            var clr = entityType.ClrType;
            var builder = modelBuilder.Entity(clr);
            
            builder.ToTable(clr.Name.Pluralize());
            
            // Unique one-translation-per-language per parent
            builder.HasIndex(nameof(ILookupTranslation.ParentId), nameof(ILookupTranslation.Language))
                .IsUnique();

            // ISO-2 language code, required
            builder.Property<string>(nameof(ILookupTranslation.Language))
                .HasMaxLength(2)
                .IsRequired();
        }
    }
}