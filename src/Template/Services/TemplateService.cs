using Avolutions.Baf.Core.Template.Abstractions;
using HandlebarsDotNet;
using Microsoft.Playwright;

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

    public async Task<byte[]> RenderPdfAsync(
        string bodyTemplate,
        object model,
        string? headerTemplate = null,
        string? footerTemplate = null,
        CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        var bodyHtml = await RenderTemplateAsync(bodyTemplate, model, ct);
        var headerHtml  = headerTemplate is null ? null : await RenderTemplateAsync(headerTemplate, model, ct);
        var footerHtml  = footerTemplate is null ? null : await RenderTemplateAsync(footerTemplate, model, ct);
        
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });
        
        var page = await browser.NewPageAsync();
        await page.SetContentAsync(bodyHtml, new PageSetContentOptions { WaitUntil = WaitUntilState.NetworkIdle });

        var pdf = await page.PdfAsync(new PagePdfOptions
        {
            Format = "A4",
            PrintBackground = true,
            DisplayHeaderFooter = headerHtml is not null || footerHtml is not null,
            HeaderTemplate = headerHtml,
            FooterTemplate = footerHtml
        });

        return pdf;
    }
}