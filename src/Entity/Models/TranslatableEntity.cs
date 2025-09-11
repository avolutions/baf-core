using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using Avolutions.Baf.Core.Entity.Abstractions;
using Avolutions.Baf.Core.Entity.Extensions;

namespace Avolutions.Baf.Core.Entity.Models;

public abstract class TranslatableEntity<TTranslation> 
    : EntityBase, ITranslatable<TTranslation>
    where TTranslation : TranslationEntity, new()
{
    protected TranslatableEntity() { }

    protected TranslatableEntity(bool createMissingTranslations)
    {
        if (createMissingTranslations)
        {
            CreateMissingTranslations();
        }
    }
    
    public ICollection<TTranslation> Translations { get; set; } = new List<TTranslation>();
    
    [NotMapped]
    public string Value => Translations.Localized(t => t.Value);

    public bool IsDefault { get; set; }

    public void CreateMissingTranslations()
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