namespace Avolutions.Baf.Core.Template.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public sealed class TemplateFieldAttribute : Attribute
{
    public string Name { get; }

    public TemplateFieldAttribute(string name)
    {
        Name = name;
    }
}