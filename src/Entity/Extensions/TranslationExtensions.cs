using System.Globalization;
using Avolutions.Baf.Core.Entity.Abstractions;

namespace Avolutions.Baf.Core.Entity.Extensions;

public static class TranslationExtensions
{
    public static string Localized<TTrans>(
        this IEnumerable<TTrans> translations,
        Func<TTrans, string?> selector)
        where TTrans : ITranslation
    {
        var language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToLowerInvariant();
        return translations.FirstOrDefault(t => t.Language.Equals(language, StringComparison.OrdinalIgnoreCase)) is { } translation
            ? selector(translation) ?? string.Empty
            : string.Empty;
    }
}