namespace Avolutions.Baf.Core.Template.Abstractions;

public interface ITemplateService
{
    Task<string> RenderTemplateFileAsync(string templatePath, object model, CancellationToken ct);
    
    Task<string> RenderTemplateAsync(string template, object model, CancellationToken ct);
    
    Task<byte[]> RenderPdfAsync(
        string bodyTemplatePath,
        object model,
        string? headerTemplatePath = null,
        string? footerTemplatePath = null,
        CancellationToken ct = default);
}