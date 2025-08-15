using Avolutions.BAF.Core.Menu.Services;

namespace Avolutions.BAF.Core.Menu.Abstractions;

public interface IMenuRegistrar<TMenu> where TMenu : IMenu
{
    int Order => 0;
    void Register(MenuProvider<TMenu> menuProvider);
}