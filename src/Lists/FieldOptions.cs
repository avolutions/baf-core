namespace Avolutions.Baf.Core.Lists;

public class FieldOptions<T>
{
    private readonly FieldDefinition<T> _definition;

    internal FieldOptions(FieldDefinition<T> definition)
    {
        _definition = definition;
    }

    public FieldOptions<T> Label(string label)
    {
        _definition.Label = label;

        return this;
    }

    public FieldOptions<T> SortBy(Func<T, object?> sort)
    {
        _definition.Sort = sort;

        return this;
    }
}