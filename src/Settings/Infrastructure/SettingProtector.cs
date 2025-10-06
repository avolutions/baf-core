using Avolutions.Baf.Core.Settings.Abstractions;
using Microsoft.AspNetCore.DataProtection;

namespace Avolutions.Baf.Core.Settings.Infrastructure;

public sealed class SettingProtector : ISettingProtector
{
    private readonly IDataProtectionProvider _provider;
    
    public SettingProtector(IDataProtectionProvider provider)
    {
        _provider = provider;
    }

    public string Protect(string plaintext, string purpose)
    {
        return _provider.CreateProtector(purpose).Protect(plaintext);
    }

    public string Unprotect(string cipher, string purpose)
    {
        return _provider.CreateProtector(purpose).Unprotect(cipher);
    }
}