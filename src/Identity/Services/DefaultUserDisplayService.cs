using Avolutions.Baf.Core.Colors.Models;
using Avolutions.Baf.Core.Identity.Abstractions;
using Avolutions.Baf.Core.Identity.Models;

namespace Avolutions.Baf.Core.Identity.Services;

public class DefaultUserDisplayService : IUserDisplayService
{
    public string GetName(User user)
    {
        return $"{user.Firstname} {user.Lastname}".Trim();
    }

    public string GetInitials(User user)
    {
        if (!string.IsNullOrWhiteSpace(user.Firstname) && !string.IsNullOrWhiteSpace(user.Lastname))
        {
            return $"{user.Firstname[0]}{user.Lastname[0]}".ToUpperInvariant();
        }

        if (!string.IsNullOrWhiteSpace(user.Firstname))
        {
            return user.Firstname[0].ToString().ToUpperInvariant();
        }

        if (!string.IsNullOrWhiteSpace(user.Lastname))
        {
            return user.Lastname[0].ToString().ToUpperInvariant();
        }

        return string.Empty;
    }

    public BafColor GetAvatarColor(User user)
    {
        return BafColor.FromBackground(user.AvatarColor);
    }
}