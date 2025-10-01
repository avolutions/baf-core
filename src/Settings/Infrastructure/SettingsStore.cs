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
    private readonly ISettingProtector _protector;

    public SettingsStore(IServiceScopeFactory scopeFactory, ISettingProtector protector)
    {
        _scopeFactory = scopeFactory;
        _protector = protector;
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
                if (!prop.CanRead || !prop.CanWrite)
                {
                    continue;
                }
                
                var key = $"{group}.{prop.Name}";
                var entry = await context.Settings.FirstOrDefaultAsync(x => x.Key == key, cancellationToken: ct);

                var isProtected = prop.PropertyType == typeof(ProtectedSetting);
                var purpose = $"{group}:{prop.Name}";

                if (entry == null)
                {
                    if (isProtected)
                    {
                        var secret = ProtectedSetting.FromPlain(string.Empty, purpose, _protector);
                        context.Settings.Add(new Setting { Key = key, Group = group, Value = secret.Cipher });
                        prop.SetValue(instance, secret);
                    }
                    else
                    {
                        var defaultValue = prop.GetValue(instance);
                        var json = JsonSerializer.Serialize(defaultValue);
                        context.Settings.Add(new Setting { Key = key, Group = group, Value = json });
                    }
                }
                else
                {
                    if (isProtected)
                    {
                        prop.SetValue(instance, new ProtectedSetting(entry.Value, purpose, _protector));
                    }
                    else
                    {
                        var value = JsonSerializer.Deserialize(entry.Value, prop.PropertyType);
                        prop.SetValue(instance, value);
                    }
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
            var isProtected = prop.PropertyType == typeof(ProtectedSetting);
            var purpose = $"{group}:{prop.Name}";
            var value = prop.GetValue(settingsInstance);

            string toStore;

            if (isProtected)
            {
                var secret = value as ProtectedSetting
                             ?? ProtectedSetting.FromPlain(string.Empty, purpose, _protector);

                toStore = secret.Cipher;
                prop.SetValue(settingsInstance, secret);
            }
            else
            {
                toStore = JsonSerializer.Serialize(value);
            }
            
            var entry = await context.Settings.FirstOrDefaultAsync(x => x.Key == key, cancellationToken: ct);
            if (entry == null)
            {
                context.Settings.Add(new Setting { Key = key, Group = group, Value = toStore });
            }
            else
            {
                entry.Value = toStore;
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