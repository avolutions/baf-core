namespace Avolutions.Baf.Core.Entity.Abstractions;

public interface ITranslatable
{
    string Value { get; }
}

public interface ITranslatable<TTranslation> : ITranslatable
    where TTranslation : ITranslation
{
    ICollection<TTranslation> Translations { get; }
}