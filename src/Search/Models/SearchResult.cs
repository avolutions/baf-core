namespace Avolutions.Baf.Core.Search.Models;

public class SearchResult
{
    public string Type { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string Text { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Url { get; set; } = string.Empty;
}