using System.Globalization;
using Avolutions.Baf.Core.Localization.Settings;
using Avolutions.Baf.Core.Settings.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Avolutions.Baf.Core.Localization;

public static class LocalizationContext
{
    private static LocalizationSettings? _settings;

    private static LocalizationSettings Settings =>
        _settings ?? throw new InvalidOperationException("LocalizationContext not initialized");

    public static IReadOnlyList<string> AvailableLanguages => Settings.AvailableLanguages;

    public static IReadOnlyList<string> AvailableCultures => Settings.AvailableCultures;

    public static string DefaultLanguage => Settings.DefaultLanguage;

    public static string DefaultCulture => Settings.DefaultCulture;

    public static string CurrentLanguage =>
        CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToLowerInvariant();

    public static void Initialize(LocalizationSettings settings)
    {
        settings.AvailableLanguages = settings.AvailableLanguages.Count == 0
            ? ["en"]
            : settings.AvailableLanguages.Select(l => l.ToLowerInvariant()).ToList();

        settings.AvailableCultures = settings.AvailableCultures.Count == 0
            ? ["en-US"]
            : settings.AvailableCultures;

        settings.DefaultLanguage = string.IsNullOrWhiteSpace(settings.DefaultLanguage)
            ? "en"
            : settings.DefaultLanguage.ToLowerInvariant();

        settings.DefaultCulture = string.IsNullOrWhiteSpace(settings.DefaultCulture)
            ? "en-US"
            : settings.DefaultCulture;

        _settings = settings;
    }

    /// <summary>
    /// Initializes from DI if it hasn't happened yet. Safe to call from anything
    /// that runs before the localization middleware is built.
    /// </summary>
    public static void EnsureInitialized(IServiceProvider provider)
    {
        if (_settings is not null)
        {
            return;
        }

        Initialize(provider.GetRequiredService<ISettings<LocalizationSettings>>().Value);
    }
}