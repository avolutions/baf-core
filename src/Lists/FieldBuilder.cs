using System.Linq.Expressions;

namespace Avolutions.Baf.Core.Lists;

public class FieldBuilder<T>
{
    private readonly List<FieldDefinition<T>> _fields = [];

    public FieldOptions<T> Field<TProperty>(Expression<Func<T, TProperty>> selector)
    {
        var path = PropertyPath.From(selector);
        var compiled = selector.Compile();

        var definition = new FieldDefinition<T>
        {
            Path = path,
            Value = item => compiled(item)
        };

        definition.Label = path;

        _fields.Add(definition);

        return new FieldOptions<T>(definition);
    }

    public IReadOnlyList<FieldDefinition<T>> Build()
    {
        return _fields;
    }
}