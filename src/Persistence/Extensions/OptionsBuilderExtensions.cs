using Avolutions.Baf.Core.Persistence.Abstractions;
using Avolutions.Baf.Core.Persistence.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Avolutions.Baf.Core.Persistence.Extensions;

public static class OptionsBuilderExtensions
{
    private static int _runnerInstalled;

    public static DbContextOptionsBuilder Seed<T, TSeedService>(this DbContextOptionsBuilder builder)
        where T : class
        where TSeedService : ISeedService<T>, new()
    {
        SeederRegistry.Add<T, TSeedService>();

        if (Interlocked.CompareExchange(ref _runnerInstalled, 1, 0) == 0)
        {
            builder
                .UseSeeding((ctx, _) =>
                {
                    foreach (var run in SeederRegistry.GetSync())
                    {
                        run(ctx);
                    }
                })
                .UseAsyncSeeding(async (ctx, _, ct) =>
                {
                    foreach (var run in SeederRegistry.GetAsync())
                    {
                        await run(ctx, ct);
                    }
                });
        }

        return builder;
    }
}