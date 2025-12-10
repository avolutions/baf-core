namespace Avolutions.Baf.Core.Entity.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class EntityRouteAttribute : Attribute
{
    public string BaseUrl { get; }

    public EntityRouteAttribute(string baseUrl)
    {
        BaseUrl = baseUrl.TrimEnd('/');
    }
}