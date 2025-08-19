using Avolutions.Baf.Core.Menu.Services;

namespace Avolutions.Baf.Core.Menu.Abstractions;

public interface IMenuRegistrar<TMenu> where TMenu : IMenu
{
    int Order => 0;
    void Register(MenuProvider<TMenu> menuProvider);
}