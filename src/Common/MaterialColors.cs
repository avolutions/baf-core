namespace Avolutions.Baf.Core.Common;

public static class MaterialColors
{
    public static readonly List<(string Background, string Text)> Colors = new()
    {
        ("#F44336", "#FFFFFF"), // Red 500
        ("#E91E63", "#FFFFFF"), // Pink 500
        ("#9C27B0", "#FFFFFF"), // Purple 500
        ("#673AB7", "#FFFFFF"), // Deep Purple 500
        ("#3F51B5", "#FFFFFF"), // Indigo 500
        ("#2196F3", "#FFFFFF"), // Blue 500
        ("#03A9F4", "#000000"), // Light Blue 500
        ("#00BCD4", "#000000"), // Cyan 500
        ("#009688", "#FFFFFF"), // Teal 500
        ("#4CAF50", "#FFFFFF"), // Green 500
        ("#8BC34A", "#000000"), // Light Green 500
        ("#CDDC39", "#000000"), // Lime 500
        ("#FFEB3B", "#000000"), // Yellow 500
        ("#FFC107", "#000000"), // Amber 500
        ("#FF9800", "#000000"), // Orange 500
        ("#FF5722", "#FFFFFF"), // Deep Orange 500
        ("#795548", "#FFFFFF"), // Brown 500
        ("#607D8B", "#FFFFFF"), // Blue Grey 500
    };
    
    public static readonly (string Background, string Text) DefaultColor = ("#9E9E9E", "#000000");
    
    public static (string Background, string Text) GetRandomColor()
    {
        var random = new Random();
        int index = random.Next(Colors.Count);
        return Colors[index];
    }
}
