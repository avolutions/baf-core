using System.Globalization;

namespace Avolutions.Baf.Core.Colors.Utilities;

public static class ColorHelper
{
    public const string LightText = "#FFFFFF";
    public const string DarkText = "#000000";

    private const double DarkTextThreshold = 0.33;

    public static string GetTextColor(string? background)
    {
        if (!TryParse(background, out var red, out var green, out var blue))
        {
            return DarkText;
        }

        if (GetRelativeLuminance(red, green, blue) > DarkTextThreshold)
        {
            return DarkText;
        }

        return LightText;
    }

    public static bool IsValid(string? color)
    {
        return TryParse(color, out _, out _, out _);
    }

    public static bool TryParse(string? color, out byte red, out byte green, out byte blue)
    {
        red = 0;
        green = 0;
        blue = 0;

        if (string.IsNullOrWhiteSpace(color))
        {
            return false;
        }

        var value = color.Trim().TrimStart('#');

        if (value.Length == 3)
        {
            value = $"{value[0]}{value[0]}{value[1]}{value[1]}{value[2]}{value[2]}";
        }

        if (value.Length != 6 && value.Length != 8)
        {
            return false;
        }

        return byte.TryParse(value.AsSpan(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out red)
            && byte.TryParse(value.AsSpan(2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out green)
            && byte.TryParse(value.AsSpan(4, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out blue);
    }

    private static double GetRelativeLuminance(byte red, byte green, byte blue)
    {
        return (0.2126 * ToLinear(red))
            + (0.7152 * ToLinear(green))
            + (0.0722 * ToLinear(blue));
    }

    private static double ToLinear(byte channel)
    {
        var value = channel / 255.0;

        if (value <= 0.04045)
        {
            return value / 12.92;
        }

        return Math.Pow((value + 0.055) / 1.055, 2.4);
    }
}