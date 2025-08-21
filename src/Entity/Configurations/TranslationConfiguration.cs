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
                     .Where(t => t.ClrType.GetInterfaces()
                         .Any(i => i.IsGenericType &&
                                   i.GetGenericTypeDefinition() == typeof(ITranslation<>))))
        {
            var clr = entityType.ClrType;
            var builder = modelBuilder.Entity(clr);
            
            builder.ToTable(clr.Name.Pluralize());
            
            // Unique one-translation-per-language per parent
            builder.HasIndex(nameof(ITranslation<object>.ParentId), nameof(ITranslation<object>.Language))
                .IsUnique();

            // ISO-2 language code, required
            builder.Property<string>(nameof(ITranslation<object>.Language))
                .HasMaxLength(2)
                .IsRequired();
        }
    }
}