namespace Avolutions.Baf.Core.Identity.Models;

public sealed record UserInfo(
    Guid Id,
    string Name,
    string Initials,
    AvatarColor AvatarColor
)
{
    public static readonly UserInfo Unknown = new(
        Guid.Empty,
        "Unknown",
        "?",
        AvatarColor.Default
    );
}