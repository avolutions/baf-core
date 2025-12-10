namespace Avolutions.Baf.Core.Lookups.Abstractions;

public interface ILookupTranslation
{
    string Language { get; set; }
    Guid ParentId { get; set; }
    string Value { get; set; }
}