namespace Avolutions.Baf.Core.Template.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class TemplateExtensionAttribute(string extension) : Attribute
{
    public string Extension { get; } = extension;
}