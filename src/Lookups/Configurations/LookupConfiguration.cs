using Avolutions.Baf.Core.Lookups.Abstractions;
using Avolutions.Baf.Core.Persistence.Abstractions;
using Humanizer;
using Microsoft.EntityFrameworkCore;

namespace Avolutions.Baf.Core.Lookups.Configurations;

public class LookupConfiguration : IModelConfiguration
{
    public void Configure(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes()
                     .Where(t => typeof(ILookup).IsAssignableFrom(t.ClrType)))
        {
            var clr = entityType.ClrType;
            var translationClr  = clr.GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ILookup<>))
                .GetGenericArguments()[0];
            
            var builder = modelBuilder.Entity(clr);
            
            builder.ToTable(clr.Name.Pluralize());
            
            builder.HasMany(translationClr, navigationName: nameof(ILookup<ILookupTranslation>.Translations))
                .WithOne()
                .HasForeignKey("ParentId")
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(nameof(ILookup.IsDefault))
                .IsUnique()
                .HasFilter("\"IsDefault\" = true");
            
            builder.Navigation(nameof(ILookup<ILookupTranslation>.Translations))
                .AutoInclude();
        }
    }
}