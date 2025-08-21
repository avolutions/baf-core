using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;
using Avolutions.Baf.Core.Settings.Abstractions;
using Avolutions.Baf.Core.Settings.Attributes;
using Avolutions.Baf.Core.Settings.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Avolutions.Baf.Core.Settings.Infrastructure;

public class SettingsStore : ISettingsStore
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ConcurrentDictionary<Type, object> _cache = new();

    public SettingsStore(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task InitializeAsync(CancellationToken ct = default)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BafDbContext>();
        
        var settingTypes = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(a => a.GetTypes())
            . Where(t => t is { IsClass: true, IsAbstract: false } &&
                         t.GetCustomAttribute<SettingsAttribute>() != null)
            .ToList();

        foreach (var type in settingTypes)
        {
            var group = type.GetCustomAttribute<SettingsAttribute>()!.Group;
            var instance = Activator.CreateInstance(type)!;
            
            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var key = $"{group}.{prop.Name}";
                var existing = await context.Settings.FirstOrDefaultAsync(x => x.Key == key, cancellationToken: ct);

                if (existing == null)
                {
                    var defaultValue = prop.GetValue(instance);
                    context.Settings.Add(new Setting
                    {
                        Key = key,
                        Value = JsonSerializer.Serialize(defaultValue),
                        Group = group
                    });
                }
                else
                {
                    var deserialized = JsonSerializer.Deserialize(existing.Value, prop.PropertyType);
                    prop.SetValue(instance, deserialized);
                }
            }
            
            Set(type, instance);
        }

        await context.SaveChangesAsync(ct);
    }

    public T Get<T>() where T : class, new()
    {
        return (T)_cache.GetOrAdd(typeof(T), _ => new T());
    }

    public async Task SaveAsync<T>(T settingsInstance, CancellationToken ct = default) where T : class, new()
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BafDbContext>();
        
        var type = typeof(T);
        var group = type.GetCustomAttribute<SettingsAttribute>()!.Group;
        
        foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (!prop.CanRead || !prop.CanWrite)
            {
                continue;
            }

            var key = $"{group}.{prop.Name}";
            var value = prop.GetValue(settingsInstance);
            var json = JsonSerializer.Serialize(value);

            var entry = await context.Settings.FirstOrDefaultAsync(x => x.Key == key, cancellationToken: ct);
            if (entry == null)
            {
                entry = new Setting
                {
                    Key = key,
                    Group = group,
                    Value = json
                };
                context.Settings.Add(entry);
            }
            else
            {
                entry.Value = json;
            }
        }

        await context.SaveChangesAsync(ct);
        Set(type, settingsInstance!);
    }

    private void Set(Type t, object instance)
    {
        _cache[t] = instance;
    }
}