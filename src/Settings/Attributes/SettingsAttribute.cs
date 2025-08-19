namespace Avolutions.Baf.Core.Settings.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class SettingsAttribute : Attribute
{
    public string Group { get; }

    public SettingsAttribute(string group)
    {
        Group = group;
    }
}
