using Avolutions.Baf.Core.Colors.Utilities;

namespace Avolutions.Baf.Core.Colors.Models;

public sealed record BafColor(string Background, string Text)
{
    public static readonly BafColor Default = FromBackground(MaterialColors.Grey);

    public static BafColor FromBackground(string? background)
    {
        if (!ColorHelper.IsValid(background))
        {
            return new BafColor(MaterialColors.Grey, ColorHelper.GetTextColor(MaterialColors.Grey));
        }

        return new BafColor(background!, ColorHelper.GetTextColor(background));
    }
    
    public string ToStyle()
    {
        return $"background-color: {Background}; color: {Text};";
    }
}