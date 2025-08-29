namespace Avolutions.Baf.Core.Entity.Abstractions;

public interface ITranslation
{
    string Language { get; set; }
    Guid ParentId { get; set; }
    string Value { get; set; }
}