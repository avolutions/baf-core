using Avolutions.Baf.Core.Persistence.Abstractions;

namespace Avolutions.Baf.Core.Persistence.Infrastructure;

using Microsoft.EntityFrameworkCore;

public static class SeederRegistry
{
    private static readonly Lock Gate = new();
    private static readonly List<Action<DbContext>> SyncSeeders = new();
    private static readonly List<Func<DbContext, CancellationToken, Task>> AsyncSeeders = new();

    public static void Add<T, TSeedService>()
        where T : class
        where TSeedService : ISeedService<T>, new()
    {
        lock (Gate)
        {
            SyncSeeders.Add(ctx =>
            {
                var seeder = new TSeedService();
                seeder.Seed(ctx);
                ctx.SaveChanges();
            });

            AsyncSeeders.Add(async (ctx, ct) =>
            {
                var seeder = new TSeedService();
                await seeder.SeedAsync(ctx, ct);
                await ctx.SaveChangesAsync(ct);
            });
        }
    }

    public static List<Action<DbContext>> GetSync()
    {
        lock (Gate) return SyncSeeders.ToList();
    }

    public static List<Func<DbContext, CancellationToken, Task>> GetAsync()
    {
        lock (Gate) return AsyncSeeders.ToList();
    }
}
