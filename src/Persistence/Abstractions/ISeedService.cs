using Microsoft.EntityFrameworkCore;

namespace Avolutions.Baf.Core.Persistence.Abstractions;

public interface ISeedService<T> where T : class
{
    bool ShouldSeed(DbContext context);
    Task<bool> ShouldSeedAsync(DbContext context, CancellationToken cancellationToken = default);
    void Seed(DbContext context);
    Task SeedAsync(DbContext context, CancellationToken cancellationToken = default);
}