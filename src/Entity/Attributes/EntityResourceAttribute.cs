namespace Avolutions.Baf.Core.Entity.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public sealed class EntityResourceAttribute : Attribute
{
    public EntityResourceAttribute(Type resourceType)
    {
        ResourceType = resourceType;
    }

    public Type ResourceType { get; }
}