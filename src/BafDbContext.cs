using Avolutions.Baf.Core.Audit.Models;
using Avolutions.Baf.Core.Identity.Models;
using Avolutions.Baf.Core.Jobs.Models;
using Avolutions.Baf.Core.NumberSequences.Models;
using Avolutions.Baf.Core.Persistence.Extensions;
using Avolutions.Baf.Core.Settings.Models;
using Avolutions.Baf.Core.Setup.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Avolutions.Baf.Core;

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
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<JobRun> JobRuns => Set<JobRun>();
    public DbSet<NumberSequence> NumberSequences => Set<NumberSequence>();
    public DbSet<Setting> Settings => Set<Setting>();
    public DbSet<SetupStatus> SetupStatus => Set<SetupStatus>();
    
    public BafDbContext(DbContextOptions options) : base(options) {}
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply any configuration from base classes
        base.OnModelCreating(modelBuilder);

        var catalog = this.GetService<BafRegistry>();
        foreach (var assembly in catalog.Assemblies)
        {
            // Model-level configs
            modelBuilder.ApplyModelConfigurationsFromAssembly(assembly);
            
            // Per-entity configs
            modelBuilder.ApplyConfigurationsFromAssembly(assembly);
        }
    }
}