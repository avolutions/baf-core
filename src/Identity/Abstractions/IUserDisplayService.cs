using Avolutions.Baf.Core.Colors.Models;
using Avolutions.Baf.Core.Identity.Models;

namespace Avolutions.Baf.Core.Identity.Abstractions;

public interface IUserDisplayService
{
    string GetName(User user);
    string GetInitials(User user);
    BafColor GetAvatarColor(User user);
}