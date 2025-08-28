namespace Avolutions.Baf.Core.Entity.Abstractions;

public interface ITranslation
{
    string Language { get; set; }
    string Value { get; set; }
}

public interface ITranslation<TParent> : ITranslation
{
    Guid ParentId { get; set; }
    TParent Parent { get; set; }
}