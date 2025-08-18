using Avolutions.BAF.Core.Settings.Abstractions;

namespace Avolutions.BAF.Core.Settings.Infrastructure;

internal sealed class SettingsAdapter<T> : ISettings<T> where T : class, new()
{
    private readonly ISettingsStore _store;
    
    public SettingsAdapter(ISettingsStore store)
    {
        _store = store;
    }

    public T Value => _store.Get<T>();
    public Task SaveAsync(T newValue, CancellationToken ct = default) => _store.SaveAsync(newValue, ct);
}