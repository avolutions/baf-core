namespace Avolutions.Baf.Core.Settings.Abstractions;

public interface ISettings<T> where T : class, new()
{
    T Value { get; }
    Task SaveAsync(T newValue, CancellationToken ct = default);
}