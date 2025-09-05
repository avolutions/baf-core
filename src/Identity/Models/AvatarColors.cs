namespace Avolutions.Baf.Core.Identity.Models;

public sealed record AvatarColor(string Background, string Text)
{
    public static readonly AvatarColor Default = new("#9E9E9E", "#000000");
}

public static class AvatarColors
{
    public static readonly List<AvatarColor> Colors =
    [
        new("#F44336", "#FFFFFF"), // Red 500
        new("#E91E63", "#FFFFFF"), // Pink 500
        new("#9C27B0", "#FFFFFF"), // Purple 500
        new("#673AB7", "#FFFFFF"), // Deep Purple 500
        new("#3F51B5", "#FFFFFF"), // Indigo 500
        new("#2196F3", "#FFFFFF"), // Blue 500
        new("#03A9F4", "#000000"), // Light Blue 500
        new("#00BCD4", "#000000"), // Cyan 500
        new("#009688", "#FFFFFF"), // Teal 500
        new("#4CAF50", "#FFFFFF"), // Green 500
        new("#8BC34A", "#000000"), // Light Green 500
        new("#CDDC39", "#000000"), // Lime 500
        new("#FFEB3B", "#000000"), // Yellow 500
        new("#FFC107", "#000000"), // Amber 500
        new("#FF9800", "#000000"), // Orange 500
        new("#FF5722", "#FFFFFF"), // Deep Orange 500
        new("#795548", "#FFFFFF"), // Brown 500
        new("#607D8B", "#FFFFFF") // Blue Grey 500
    ];
    
    public static readonly AvatarColor Default = new ("#9E9E9E", "#000000");

    public static AvatarColor GetRandom()
    {
        var random = new Random();
        return Colors[random.Next(Colors.Count)];
    }
}
