namespace Avolutions.Baf.Core.Settings.Abstractions;

public interface ISettingProtector
{
    string Protect(string plaintext, string purpose);
    string Unprotect(string cipher, string purpose);
}