using Avolutions.Baf.Core.Entity.Abstractions;
using Avolutions.Baf.Core.Localization;

namespace Avolutions.Baf.Core.Entity.Extensions;

public static class TranslationExtensions
{
    public static string Localized<TTrans>(
        this ICollection<TTrans> translations,
        Func<TTrans, string?> selector)
        where TTrans : ITranslation
    {
        var translation = translations.FirstOrDefault(t => t.Language == LocalizationContext.CurrentLanguage)
                          ?? translations.FirstOrDefault(t => t.Language == LocalizationContext.DefaultLanguage);
        
        return translation is not null ? selector(translation) ?? string.Empty : string.Empty;
    }
}