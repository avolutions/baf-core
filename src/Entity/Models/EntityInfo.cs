using System.Globalization;

namespace Avolutions.Baf.Core.Entity.Models;

public sealed record EntityInfo(
    string Key,
    Type ClrType,
    Type ResourceType,
    IReadOnlyDictionary<string, EntityNames> NamesByCulture)
{
    public EntityNames Names => Resolve(CultureInfo.CurrentUICulture);

    public string Singular => Names.Singular;

    public string Plural => Names.Plural;

    private EntityNames Resolve(CultureInfo culture)
    {
        while (!string.IsNullOrEmpty(culture.Name))
        {
            if (NamesByCulture.TryGetValue(culture.Name, out var names))
            {
                return names;
            }

            culture = culture.Parent;
        }

        return NamesByCulture.Values.FirstOrDefault() ?? new EntityNames(Key, Key);
    }
}