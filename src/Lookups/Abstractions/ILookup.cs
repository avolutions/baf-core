namespace Avolutions.Baf.Core.Lookups.Abstractions;

public interface ILookup
{
    string Value { get; }
    bool IsDefault { get; set; }
}

public interface ILookup<TTranslation> : ILookup
    where TTranslation : ILookupTranslation
{
    ICollection<TTranslation> Translations { get; }
}