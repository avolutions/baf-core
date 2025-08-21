using Avolutions.Baf.Core.Entity.Abstractions;

namespace Avolutions.Baf.Core.Entity.Models;

public abstract class TranslationEntity<TParent>
    : EntityBase, ITranslation<TParent>
{
    public Guid ParentId { get; set; }
    public TParent Parent { get; set; } = default!;
    public string Language { get; set; } = default!;
    public string Value { get; set; } = default!;
}