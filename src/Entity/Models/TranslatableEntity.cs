using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using Avolutions.Baf.Core.Entity.Abstractions;

namespace Avolutions.Baf.Core.Entity.Models;

public abstract class TranslatableEntity<TSelf, TTranslation> 
    : EntityBase, ITranslatable<TSelf, TTranslation>
    where TSelf : TranslatableEntity<TSelf, TTranslation>
    where TTranslation : TranslationEntity<TSelf>
{
    public required string Key { get; set; }
    public ICollection<TTranslation> Translations { get; set; } = new List<TTranslation>();
    
    [NotMapped]
    public string Value
    {
        get
        {
            var lang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToLowerInvariant();

            return Translations.FirstOrDefault(t => t.Language.Equals(lang, StringComparison.OrdinalIgnoreCase))?.Value
                   ?? Key;
        }
    }
}