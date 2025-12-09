using System.ComponentModel.DataAnnotations.Schema;
using Avolutions.Baf.Core.Entity.Models;
using Avolutions.Baf.Core.Localization;
using Avolutions.Baf.Core.Lookups.Abstractions;
using Avolutions.Baf.Core.Lookups.Extensions;

namespace Avolutions.Baf.Core.Lookups.Models;

public abstract class Lookup<TTranslation> 
    : EntityBase, ILookup<TTranslation>
    where TTranslation : LookupTranslation, new()
{
    protected Lookup() { }

    protected Lookup(bool createMissingTranslations)
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
        foreach (var language in LocalizationContext.AvailableLanguages)
        {
            if (!Translations.Any(t => t.Language == language))
            {
                Translations.Add(new TTranslation { Language = language });
            }
        }
    }
}