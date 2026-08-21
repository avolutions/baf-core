using System.Linq.Expressions;

namespace Avolutions.Baf.Core.Lists;

public static class PropertyPath
{
    public static string From<T, TProperty>(Expression<Func<T, TProperty>> selector)
    {
        var expression = selector.Body;

        if (expression is UnaryExpression unary)
        {
            expression = unary.Operand;
        }

        var parts = new List<string>();

        while (expression is MemberExpression member)
        {
            parts.Insert(0, member.Member.Name);
            expression = member.Expression!;
        }

        if (parts.Count == 0)
        {
            throw new ArgumentException("Selector must point to a property.", nameof(selector));
        }

        return string.Join(".", parts);
    }
}