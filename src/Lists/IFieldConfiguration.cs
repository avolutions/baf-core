namespace Avolutions.Baf.Core.Lists;

public interface IFieldConfiguration<T>
{
    void Configure(FieldBuilder<T> builder);
}