using Avolutions.Baf.Core.Entity.Abstractions;
using Avolutions.Baf.Core.Persistence.Abstractions;
using Humanizer;
using Microsoft.EntityFrameworkCore;

namespace Avolutions.Baf.Core.Entity.Configurations;

public class TranslatableConfiguration : IModelConfiguration
{
    public void Configure(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes()
                     .Where(t => typeof(ITranslatable).IsAssignableFrom(t.ClrType)))
        {
            var clr = entityType.ClrType;
            var builder = modelBuilder.Entity(clr);
            
            builder.ToTable(clr.Name.Pluralize());
        }
    }
}