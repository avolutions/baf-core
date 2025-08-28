using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using Avolutions.Baf.Core.Entity.Abstractions;
using Avolutions.Baf.Core.Entity.Extensions;

namespace Avolutions.Baf.Core.Entity.Models;

public abstract class TranslatableEntity<TSelf, TTranslation> 
    : EntityBase, ITranslatable<TSelf, TTranslation>
    where TSelf : TranslatableEntity<TSelf, TTranslation>
    where TTranslation : TranslationEntity<TSelf>
{
    public ICollection<TTranslation> Translations { get; set; } = new List<TTranslation>();
    
    [NotMapped]
    public string Value => Translations.Localized(t => t.Value);
    
    public void CreateDefaultTranslation()
    {
        if (Translations.Count > 0)
        {
            return;
        }

        var defaultTranslation = new TTranslation
        {
            Language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToLowerInvariant()
        };

        Translations.Add(defaultTranslation);
    }
}