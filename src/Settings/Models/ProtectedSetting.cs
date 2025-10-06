using Avolutions.Baf.Core.Settings.Abstractions;

namespace Avolutions.Baf.Core.Settings.Models;

public sealed class ProtectedSetting
{
    public string Cipher { get; }
    public string Purpose { get; }

    private readonly ISettingProtector _protector;

    public ProtectedSetting(string cipher, string purpose, ISettingProtector protector)
    {
        Cipher = cipher ?? string.Empty;
        Purpose = purpose ?? throw new ArgumentNullException(nameof(purpose));
        _protector = protector ?? throw new ArgumentNullException(nameof(protector));
    }

    public static ProtectedSetting FromPlain(string plain, string purpose, ISettingProtector protector)
    {
        return new ProtectedSetting(protector.Protect(plain ?? string.Empty, purpose), purpose, protector);
    }

    public T Use<T>(Func<string, T> action)
    {
        var plain = _protector.Unprotect(Cipher, Purpose);
        return action(plain);
    }

    public void Use(Action<string> action)
    {
        var plain = _protector.Unprotect(Cipher, Purpose);
        action(plain);
    }
    
    public async Task UseAsync(Func<string, Task> action)
    {
        var plain = _protector.Unprotect(Cipher, Purpose);
        await action(plain);
    }

    public async Task<T> UseAsync<T>(Func<string, Task<T>> action)
    {
        var plain = _protector.Unprotect(Cipher, Purpose);
        return await action(plain);
    }
}