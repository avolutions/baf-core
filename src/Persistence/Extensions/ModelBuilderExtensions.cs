using System.Reflection;
using Avolutions.Baf.Core.Persistence.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Avolutions.Baf.Core.Persistence.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyModelConfigurationsFromAssembly(
        this ModelBuilder modelBuilder,
        Assembly assembly)
    {
        var types = assembly
            .GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface && typeof(IModelConfiguration).IsAssignableFrom(t));

        foreach (var t in types)
        {
            var constructor = t.GetConstructor(Type.EmptyTypes);
            if (constructor is null)
            {
                continue;
            }

            if (Activator.CreateInstance(t) is IModelConfiguration modelConfiguration)
            {
                modelConfiguration.Configure(modelBuilder);
            }
        }
    }
}