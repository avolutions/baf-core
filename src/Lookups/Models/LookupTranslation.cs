using Avolutions.Baf.Core.Entity.Models;
using Avolutions.Baf.Core.Lookups.Abstractions;

namespace Avolutions.Baf.Core.Lookups.Models;

public abstract class LookupTranslation
    : EntityBase, ILookupTranslation
{
    public Guid ParentId { get; set; }
    public string Language { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}