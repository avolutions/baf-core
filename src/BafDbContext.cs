using Microsoft.EntityFrameworkCore;

namespace Avolutions.BAF.Core;

/// <summary>
/// Base DbContext for the BAF framework.
/// 
/// - Applies all entity configurations from the BAF.Core assembly
///   so modules (in subfolders) automatically register their entities.
/// - Sets the database schema to "baf" for all entities that belong
///   to this assembly, keeping them separate from the application’s own tables.
/// 
/// This class is meant to be inherited by the application's DbContext,
/// so that migrations are generated in the application project while still
/// including all BAF tables.
/// </summary>
public class BafDbContext : DbContext
{
    /// <summary>
    /// Protected constructor so only derived contexts (e.g. the app's DbContext)
    /// can create an instance. Accepts DbContextOptions to support DI and provider configuration.
    /// </summary>
    protected BafDbContext(DbContextOptions options) : base(options) { }

    /// <summary>
    /// Configures the EF Core model:
    /// 1. Applies configurations from the current assembly (BAF modules).
    /// 2. Sets schema "baf" for all entities that belong to this assembly.
    /// Application-specific entities remain in their own/default schema.
    /// </summary>
    /// <param name="modelBuilder">The model builder used to configure entity mappings.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply any configuration from base classes
        base.OnModelCreating(modelBuilder);
        
        // Get the assembly that contains this BafDbContext and all BAF modules
        // and apply all IEntityTypeConfiguration<> implementations from this assembly
        var bafAssembly = typeof(BafDbContext).Assembly;
        modelBuilder.ApplyConfigurationsFromAssembly(bafAssembly);
        
        // Iterate over all entity types in the model
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clr = entityType.ClrType;
            
            // Only process entities that belong to the BAF assembly
            if (clr.Assembly == bafAssembly)
            {
                // Set the schema for this entity's table to "baf"
                // This groups all BAF tables in their own schema, separate from application tables
                entityType.SetSchema("baf");
            }
        }
    }
}