using Avolutions.Baf.Core.Localization;
using Avolutions.Baf.Core.Lookups.Abstractions;

namespace Avolutions.Baf.Core.Lookups.Extensions;

public static class LookupTranslationExtensions
{
    public static string Localized<TTrans>(
        this ICollection<TTrans> translations,
        Func<TTrans, string?> selector)
        where TTrans : ILookupTranslation
    {
        var translation = translations.FirstOrDefault(t => t.Language == LocalizationContext.CurrentLanguage)
                          ?? translations.FirstOrDefault(t => t.Language == LocalizationContext.DefaultLanguage);
        
        return translation is not null ? selector(translation) ?? string.Empty : string.Empty;
    }
}