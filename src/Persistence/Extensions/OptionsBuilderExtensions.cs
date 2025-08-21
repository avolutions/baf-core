using Avolutions.Baf.Core.Persistence.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Avolutions.Baf.Core.Persistence.Extensions;

public static class OptionsBuilderExtensions
{
    public static DbContextOptionsBuilder Seed<T, TSeedService>(this DbContextOptionsBuilder optionsBuilder)
        where T : class
        where TSeedService : ISeedService<T>, new()
    {
        optionsBuilder
            .UseSeeding((context, _) =>
            {
                var seeder = new TSeedService();
                seeder.Seed(context);
                context.SaveChanges();
            })
            .UseAsyncSeeding(async (context, _, cancellationToken) =>
            {
                var seeder = new TSeedService();
                await seeder.SeedAsync(context, cancellationToken);
                await context.SaveChangesAsync(cancellationToken);
            });

        return optionsBuilder;
    }
}