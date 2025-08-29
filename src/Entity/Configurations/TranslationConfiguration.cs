using Avolutions.Baf.Core.Entity.Abstractions;
using Avolutions.Baf.Core.Persistence.Abstractions;
using Humanizer;
using Microsoft.EntityFrameworkCore;

namespace Avolutions.Baf.Core.Entity.Configurations;

public class TranslationConfiguration : IModelConfiguration
{
    public void Configure(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes()
                     .Where(t => typeof(ITranslation).IsAssignableFrom(t.ClrType)))
        {
            var clr = entityType.ClrType;
            var builder = modelBuilder.Entity(clr);
            
            builder.ToTable(clr.Name.Pluralize());
            
            // Unique one-translation-per-language per parent
            builder.HasIndex(nameof(ITranslation.ParentId), nameof(ITranslation.Language))
                .IsUnique();

            // ISO-2 language code, required
            builder.Property<string>(nameof(ITranslation.Language))
                .HasMaxLength(2)
                .IsRequired();
        }
    }
}