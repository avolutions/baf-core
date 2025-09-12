namespace Avolutions.Baf.Core.Entity.Abstractions;

public interface ITranslatable
{
    string Value { get; }
    bool IsDefault { get; set; }
}

public interface ITranslatable<TTranslation> : ITranslatable
    where TTranslation : ITranslation
{
    ICollection<TTranslation> Translations { get; }
}