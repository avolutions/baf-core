using System.Globalization;
using Avolutions.Baf.Core.Localization.Settings;

namespace Avolutions.Baf.Core.Localization;

public static class LocalizationContext
{
    private static IReadOnlyList<string>? _availableLanguages;
    private static string? _defaultLanguage;
    
    public static IReadOnlyList<string> AvailableLanguages => 
        _availableLanguages ?? throw new InvalidOperationException("LocalizationContext not initialized");
    
    public static string DefaultLanguage => 
        _defaultLanguage ?? throw new InvalidOperationException("LocalizationContext not initialized");
    
    public static string CurrentLanguage => 
        CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToLowerInvariant();
    
    public static void Initialize(LocalizationSettings settings)
    {
        _availableLanguages = settings.AvailableLanguages
            .Select(l => l.ToLowerInvariant())
            .ToList();
        _defaultLanguage = settings.DefaultLanguage.ToLowerInvariant();
    }
}