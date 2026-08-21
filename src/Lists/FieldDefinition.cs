namespace Avolutions.Baf.Core.Lists;

public class FieldDefinition<T>
{
    public required string Path { get; init; }

    public string Label { get; internal set; } = string.Empty;

    public required Func<T, object?> Value { get; init; }

    public Func<T, object?>? Sort { get; internal set; }

    /// <summary>Runtime state, controlled by the column selector. Not configuration.</summary>
    public bool IsVisible { get; set; } = true;

    public string GetText(T item)
    {
        return Value(item)?.ToString() ?? string.Empty;
    }

    public Func<T, object> GetSortBy()
    {
        var selector = Sort ?? Value;

        return item => selector(item) ?? string.Empty;
    }

    internal FieldDefinition<T> Clone()
    {
        return new FieldDefinition<T>
        {
            Path = Path,
            Label = Label,
            Value = Value,
            Sort = Sort,
            IsVisible = true
        };
    }
}