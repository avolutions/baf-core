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
        string bodyTemplatePath,
        object model,
        string? headerTemplatePath = null,
        string? footerTemplatePath = null,
        CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        var bodyHtml = await RenderTemplateFileAsync(bodyTemplatePath, model, ct);
        var headerHtml  = headerTemplatePath is null ? null : await RenderTemplateFileAsync(headerTemplatePath, model, ct);
        var footerHtml  = footerTemplatePath is null ? null : await RenderTemplateFileAsync(footerTemplatePath, model, ct);
        
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
            FooterTemplate = footerHtml,
            Margin = new Margin
            {
                Top = "30mm",
                Bottom = "30mm",
                Left = "12mm",
                Right = "12mm"
            }
        });

        return pdf;
    }
}