namespace Avolutions.Baf.Core.Menu.Models;

public class MenuItem
{
    public string Id { get; set; } = default!;
    public string? ParentId { get; set; }
    public string? Url { get; set; }
    public string? Icon { get; set; }
    public int Order { get; set; } = 0;
    public string? RequiredRole { get; set; }
    
    public Func<string>? GetTitle { get; set; }
    public string Title => GetTitle?.Invoke() ?? Id;

    public List<MenuItem> Children { get; } = new();
}