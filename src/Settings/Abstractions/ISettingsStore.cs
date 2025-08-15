namespace Avolutions.BAF.Core.Settings.Abstractions;

public interface ISettingsStore
{
    Task InitializeAsync(CancellationToken ct = default);
    T Get<T>() where T : class, new();
    Task SaveAsync<T>(T value, CancellationToken ct = default) where T : class, new();
}