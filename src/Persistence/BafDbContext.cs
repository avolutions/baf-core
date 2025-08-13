using Avolutions.BAF.Core.Identity.Models;
using Avolutions.BAF.Core.Persistence.Abstractions;
using Avolutions.BAF.Core.Persistence.Extensions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

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
    public BafDbContext(DbContextOptions options) : base(options) {}

    public override int SaveChanges()
    {
        return SaveChangesAsync().GetAwaiter().GetResult();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {   
        var hooks = (this.GetService<IEnumerable<ISaveChangesHook>>() ?? [])
            .OrderBy(h => h.Order)
            .ThenBy(h => h.GetType().FullName)
            .ToArray();
        
        foreach (var hook in hooks)
        {
            await hook.OnBeforeSaveChanges(this, cancellationToken);
        }
        
        var rows = await base.SaveChangesAsync(cancellationToken);
        
        foreach (var hook in hooks.Reverse())
        {
            await hook.OnAfterSaveChanges(this, rows, cancellationToken);
        }
        
        return rows;
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply any configuration from base classes
        base.OnModelCreating(modelBuilder);
        
        var bafAssembly = typeof(BafDbContext).Assembly;
        
        // Model-level configs
        modelBuilder.ApplyModelConfigurationsFromAssembly(bafAssembly);

        // Per-entity configs
        modelBuilder.ApplyConfigurationsFromAssembly(bafAssembly);
    }
}