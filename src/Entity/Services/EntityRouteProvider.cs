using System.Reflection;
using Avolutions.Baf.Core.Entity.Abstractions;
using Avolutions.Baf.Core.Entity.Attributes;
using Humanizer;

namespace Avolutions.Baf.Core.Entity.Services;

public class EntityRouteProvider<TEntity> : IEntityRouteProvider<TEntity>
    where TEntity : class, IEntity
{
    private readonly string _baseUrl;

    public EntityRouteProvider()
    {
        var attribute = typeof(TEntity).GetCustomAttribute<EntityRouteAttribute>();

        _baseUrl = attribute?.BaseUrl ?? $"/{typeof(TEntity).Name.Pluralize().Kebaberize()}";
    }

    public string Index => _baseUrl;
    public string Create => $"{_baseUrl}/create";
    public string Details(Guid id) => $"{_baseUrl}/{id}";
    public string Edit(Guid id) => $"{_baseUrl}/edit/{id}";
}