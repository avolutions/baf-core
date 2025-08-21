namespace Avolutions.Baf.Core.Entity.Abstractions;

public interface ITranslation<TParent>
{
    Guid ParentId { get; set; }
    TParent Parent { get; set; }
    string Language { get; set; }
    string Value { get; set; }
}