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
    
    public static void Initialize(LocalizationSettings settings)
    {
        _availableLanguages = settings.AvailableLanguages;
        _defaultLanguage = settings.DefaultLanguage;
    }
}