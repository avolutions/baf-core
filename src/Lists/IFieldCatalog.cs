using System.Linq.Expressions;

namespace Avolutions.Baf.Core.Lists;

public interface IFieldCatalog<T>
{
    IReadOnlyList<FieldDefinition<T>> GetFields();
    IReadOnlyList<FieldDefinition<T>> GetFields(params Expression<Func<T, object?>>[] visible);
}