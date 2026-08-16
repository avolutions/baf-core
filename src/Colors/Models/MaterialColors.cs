namespace Avolutions.Baf.Core.Colors.Models;

public static class MaterialColors
{
    public const string Grey = "#9E9E9E";

    public static readonly IReadOnlyList<string> Palette =
    [
        "#F44336", // Red 500
        "#E91E63", // Pink 500
        "#9C27B0", // Purple 500
        "#673AB7", // Deep Purple 500
        "#3F51B5", // Indigo 500
        "#2196F3", // Blue 500
        "#03A9F4", // Light Blue 500
        "#00BCD4", // Cyan 500
        "#009688", // Teal 500
        "#4CAF50", // Green 500
        "#8BC34A", // Light Green 500
        "#CDDC39", // Lime 500
        "#FFEB3B", // Yellow 500
        "#FFC107", // Amber 500
        "#FF9800", // Orange 500
        "#FF5722", // Deep Orange 500
        "#795548", // Brown 500
        "#607D8B"  // Blue Grey 500
    ];

    public static readonly IReadOnlyList<string> DarkPalette =
    [
        "#D32F2F", // Red 700
        "#C2185B", // Pink 700
        "#7B1FA2", // Purple 700
        "#512DA8", // Deep Purple 700
        "#303F9F", // Indigo 700
        "#1976D2", // Blue 700
        "#0288D1", // Light Blue 700
        "#0097A7", // Cyan 700
        "#00796B", // Teal 700
        "#388E3C", // Green 700
        "#689F38", // Light Green 700
        "#AFB42B", // Lime 700
        "#FBC02D", // Yellow 700
        "#FFA000", // Amber 700
        "#F57C00", // Orange 700
        "#E64A19", // Deep Orange 700
        "#5D4037", // Brown 700
        "#455A64"  // Blue Grey 700
    ];

    public static string GetRandom(IReadOnlyList<string>? palette = null)
    {
        var source = palette ?? Palette;

        return source[Random.Shared.Next(source.Count)];
    }
}