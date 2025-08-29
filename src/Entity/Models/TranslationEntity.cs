using Avolutions.Baf.Core.Entity.Abstractions;

namespace Avolutions.Baf.Core.Entity.Models;

public abstract class TranslationEntity
    : EntityBase, ITranslation
{
    public Guid ParentId { get; set; }
    public string Language { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}