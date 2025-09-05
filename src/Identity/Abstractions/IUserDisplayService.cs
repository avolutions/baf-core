using Avolutions.Baf.Core.Identity.Models;

namespace Avolutions.Baf.Core.Identity.Abstractions;

public interface IUserDisplayService
{
    string GetName(User user);
    string GetInitials(User user);
    AvatarColor GetAvatarColor(User user);
}