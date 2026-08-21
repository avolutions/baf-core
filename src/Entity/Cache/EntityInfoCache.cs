using System.Globalization;
using System.Reflection;
using Avolutions.Baf.Core.Caching;
using Avolutions.Baf.Core.Entity.Abstractions;
using Avolutions.Baf.Core.Entity.Attributes;
using Avolutions.Baf.Core.Entity.Models;
using Avolutions.Baf.Core.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace Avolutions.Baf.Core.Entity.Cache;

public class EntityInfoCache : CacheBase<string, EntityInfo>, IEntityInfoCache
{
    private const string SingularKey = "Name.Singular";
    private const string PluralKey = "Name.Plural";

    public EntityInfoCache(IServiceScopeFactory scopeFactory) : base(scopeFactory)
    {
    }

    protected override IEqualityComparer<string> KeyComparer => StringComparer.OrdinalIgnoreCase;

    protected override Task<IReadOnlyList<EntityInfo>> LoadAsync(
        CancellationToken cancellationToken)
    {
        using var scope = ScopeFactory.CreateScope();

        LocalizationContext.EnsureInitialized(scope.ServiceProvider);

        var factory = scope.ServiceProvider.GetRequiredService<IStringLocalizerFactory>();
        var registry = scope.ServiceProvider.GetRequiredService<BafRegistry>();

        var items = new List<EntityInfo>();

        foreach (var assembly in registry.ModuleAssemblies)
        {
            foreach (var type in GetTypes(assembly))
            {
                if (type is not { IsClass: true, IsAbstract: false })
                {
                    continue;
                }

                if (!typeof(IEntity).IsAssignableFrom(type))
                {
                    continue;
                }

                var attribute = type.GetCustomAttribute<EntityResourceAttribute>();

                if (attribute is null)
                {
                    continue;
                }

                items.Add(new EntityInfo(
                    type.Name,
                    type,
                    attribute.ResourceType,
                    LoadNames(factory, attribute.ResourceType, type.Name)));
            }
        }

        return Task.FromResult<IReadOnlyList<EntityInfo>>(items);
    }
    
    private static IEnumerable<Type> GetTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException exception)
        {
            return exception.Types.Where(type => type is not null)!;
        }
    }

    protected override string GetId(EntityInfo item) => item.Key;

    private static IReadOnlyDictionary<string, EntityNames> LoadNames(
        IStringLocalizerFactory factory,
        Type resourceType,
        string fallback)
    {
        var localizer = factory.Create(resourceType);
        var previous = CultureInfo.CurrentUICulture;
        var names = new Dictionary<string, EntityNames>(StringComparer.OrdinalIgnoreCase);

        try
        {
            foreach (var language in LocalizationContext.AvailableLanguages)
            {
                var culture = CultureInfo.GetCultureInfo(language);
                CultureInfo.CurrentUICulture = culture;

                names[culture.Name] = new EntityNames(
                    Read(localizer, SingularKey, fallback),
                    Read(localizer, PluralKey, fallback));
            }
        }
        finally
        {
            CultureInfo.CurrentUICulture = previous;
        }

        return names;
    }

    private static string Read(IStringLocalizer localizer, string key, string fallback)
    {
        var value = localizer[key];

        return value.ResourceNotFound ? fallback : value.Value;
    }
}