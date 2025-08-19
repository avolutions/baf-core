using System.Security.Claims;
using Avolutions.Baf.Core.Menu.Abstractions;
using Avolutions.Baf.Core.Menu.Models;
using Mapster;

namespace Avolutions.Baf.Core.Menu.Services;

public class MenuProvider<TMenu> where TMenu : IMenu
{
    private readonly List<MenuItem> _menuItems = new();

    public MenuProvider(IEnumerable<IMenuRegistrar<TMenu>> contributors)
    {
        foreach (var contributor in contributors.OrderBy(c => c.Order))
        {
            contributor.Register(this);
        } 
    }

    public void Add(MenuItem menuItem)
    {
        _menuItems.Add(menuItem);
    }

    public List<MenuItem> GetMenuItemsVisibleToUser(ClaimsPrincipal user)
    {
        // Create a copy of the original list to not mutate it for other users with different permissions
        var menuItems = _menuItems.Adapt<List<MenuItem>>();
        
        // Filter items that are visible to the current user
        var filteredItems = menuItems
            .Where(item => string.IsNullOrEmpty(item.RequiredRole) || user.IsInRole(item.RequiredRole))
            .ToList();

        var filteredLookup = filteredItems.ToDictionary(x => x.Id);
        var rootItems = new List<MenuItem>();
        
        // Rebuild relationships only with filtered items
        foreach (var item in filteredItems)
        {
            if (item.ParentId is null)
            {
                rootItems.Add(item);
            }
            else if (filteredLookup.TryGetValue(item.ParentId, out var parent))
            {
                parent.Children.Add(item);
            }
        }

        SortItems(rootItems);
        
        return rootItems;
    }

    private static void SortItems(List<MenuItem> menuItems)
    {
        menuItems.Sort((a, b) => a.Order.CompareTo(b.Order));
        foreach (var menuItem in menuItems)
        {
            SortItems(menuItem.Children);
        }
    }
}