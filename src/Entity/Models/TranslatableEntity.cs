using System.ComponentModel.DataAnnotations.Schema;
using Avolutions.Baf.Core.Entity.Abstractions;
using Avolutions.Baf.Core.Entity.Extensions;
using Avolutions.Baf.Core.Localization;

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
        foreach (var availableLanguage in LocalizationContext.AvailableLanguages)
        {
            if (!Translations.Any(t => t.Language == availableLanguage))
            {
                Translations.Add(new TTranslation { Language = availableLanguage });
            }
        }
    }
}