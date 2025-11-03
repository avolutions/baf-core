namespace Avolutions.Baf.Core.Template.Abstractions;

public interface ITemplateService
{
    Task<string> RenderTemplateFileAsync(string templatePath, object model, CancellationToken ct);
    
    Task<string> RenderTemplateAsync(string template, object model, CancellationToken ct);
}