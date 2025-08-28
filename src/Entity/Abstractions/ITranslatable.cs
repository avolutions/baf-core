namespace Avolutions.Baf.Core.Entity.Abstractions;

public interface ITranslatable
{
}

public interface ITranslatable<TSelf, TTranslation> : ITranslatable
    where TSelf : ITranslatable<TSelf, TTranslation>
    where TTranslation : ITranslation<TSelf>
{
    ICollection<TTranslation> Translations { get; }
    string Value { get; }
}