using Avolutions.BAF.Core.Identity.Models;
using Avolutions.BAF.Core.Persistence.Abstractions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Avolutions.BAF.Core.Persistence;

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
public class BafDbContext : IdentityDbContext<User, Role, Guid>
{
    private readonly IReadOnlyList<IModelCreatingHook> _modelHooks;
    private readonly IReadOnlyList<ISaveChangesHook> _saveHooks;
    
    /// <summary>
    /// Protected constructor so only derived contexts (e.g. the app's DbContext)
    /// can create an instance. Accepts DbContextOptions to support DI and provider configuration.
    /// </summary>
    protected BafDbContext(
        DbContextOptions options,
        IEnumerable<IModelCreatingHook>? modelHooks = null,
        IEnumerable<ISaveChangesHook>? saveHooks = null) : base(options)
    {
        _modelHooks = (modelHooks ?? [])
            .OrderBy(h => h.Order)
            .ToList();

        _saveHooks = (saveHooks ?? [])
            .OrderBy(h => h.Order)
            .ToList();
    }

    public override int SaveChanges() => SaveChangesAsync().GetAwaiter().GetResult();
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var hook in _saveHooks)
        {
            await hook.OnSavingAsync(this, cancellationToken);
        }
        
        var rows = await base.SaveChangesAsync(cancellationToken);

        foreach (var hook in _saveHooks.Reverse())
        {
            await hook.OnSavedAsync(this, rows, cancellationToken);
        }
        
        return rows;
    }
    
    /// <summary>
    /// Configures the EF Core model: by applying configurations from the current assembly (BAF modules).
    /// Application-specific entities remain in their own/default schema.
    /// </summary>
    /// <param name="modelBuilder">The model builder used to configure entity mappings.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply any configuration from base classes
        base.OnModelCreating(modelBuilder);
        
        // Apply all IEntityTypeConfiguration<> implementations from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BafDbContext).Assembly);
        
        foreach (var hook in _modelHooks)
        {
            hook.Configure(modelBuilder);
        }
    }
}