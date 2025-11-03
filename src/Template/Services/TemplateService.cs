using Avolutions.Baf.Core.Template.Abstractions;
using HandlebarsDotNet;

namespace Avolutions.Baf.Core.Template.Services;

public class TemplateService : ITemplateService
{
    public async Task<string> RenderTemplateFileAsync(string templatePath, object model, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        if (!Path.IsPathRooted(templatePath))
        {
            templatePath = Path.Combine(AppContext.BaseDirectory, templatePath);
        }

        if (!File.Exists(templatePath))
        {
            throw new FileNotFoundException($"Template not found: {templatePath}", templatePath);
        }

        var source = await File.ReadAllTextAsync(templatePath, ct);
        
        return await RenderTemplateAsync(source, model, ct);
    }

    public async Task<string> RenderTemplateAsync(string template, object model, CancellationToken ct)
    {
        var compiledTemplate = Handlebars.Compile(template);
        
        return await Task.FromResult(compiledTemplate(model));
    }
}