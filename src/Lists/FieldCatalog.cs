using System.Linq.Expressions;

namespace Avolutions.Baf.Core.Lists;

public class FieldCatalog<T> : IFieldCatalog<T>
{
    private readonly IReadOnlyList<FieldDefinition<T>> _fields;

    public FieldCatalog(IFieldConfiguration<T> configuration)
    {
        var builder = new FieldBuilder<T>();
        configuration.Configure(builder);

        _fields = builder.Build();
    }

    public IReadOnlyList<FieldDefinition<T>> GetFields()
    {
        return _fields.Select(field => field.Clone()).ToList();
    }

    public IReadOnlyList<FieldDefinition<T>> GetFields(params Expression<Func<T, object?>>[] visible)
    {
        var fields = GetFields();
        var paths = visible.Select(PropertyPath.From).ToHashSet();

        foreach (var path in paths)
        {
            if (fields.All(field => field.Path != path))
            {
                throw new InvalidOperationException(
                    $"No field '{path}' is configured for {typeof(T).Name}.");
            }
        }

        foreach (var field in fields)
        {
            field.IsVisible = paths.Contains(field.Path);
        }

        return fields;
    }
}