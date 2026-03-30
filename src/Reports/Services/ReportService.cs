using Avolutions.Baf.Core.Reports.Abstractions;
using Avolutions.Baf.Core.Template.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;

namespace Avolutions.Baf.Core.Reports.Services;

public class ReportService : IReportService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly HandlebarsTemplateService _templateService;
    
    public ReportService(IServiceProvider serviceProvider, HandlebarsTemplateService templateService) 
    {
        _templateService = templateService;
        _serviceProvider = serviceProvider;
    }

    public async Task<byte[]> RenderPdfAsync<TReport>(IReportArgs args, CancellationToken ct = default)
        where TReport : class, IReport
    {
        var report = ActivatorUtilities.CreateInstance<TReport>(_serviceProvider);
        var model = await report.BuildModelAsync(args, ct);
        
        var contentHtml = await RenderTemplateAsync(report.ContentTemplatePath, model, ct);
        var headerHtml = report.HeaderTemplatePath != null
            ? await RenderTemplateAsync(report.HeaderTemplatePath, model, ct)
            : "<span></span>";
        var footerHtml = report.FooterTemplatePath != null
            ? await RenderTemplateAsync(report.FooterTemplatePath, model, ct)
            : "<span></span>";
        
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });
        
        var page = await browser.NewPageAsync();
        await page.SetContentAsync(contentHtml, new PageSetContentOptions { WaitUntil = WaitUntilState.NetworkIdle });

        var pdf = await page.PdfAsync(new PagePdfOptions
        {
            Format = "A4",
            PrintBackground = true,
            DisplayHeaderFooter = true,
            HeaderTemplate = headerHtml,
            FooterTemplate = footerHtml,
            Margin = new Margin
            {
                Top = "30mm",
                Bottom = "30mm",
                Left = "10mm",
                Right = "10mm"
            }
        });

        return pdf;
    }
    
    private async Task<string> RenderTemplateAsync(string templatePath, IReportModel model, CancellationToken ct)
    {
        var fullPath = Path.IsPathRooted(templatePath)
            ? templatePath
            : Path.Combine(AppContext.BaseDirectory, templatePath);

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException($"Template not found: {fullPath}", fullPath);
        }

        var template = await File.ReadAllTextAsync(fullPath, ct);
        return await _templateService.ApplyModelToTemplateAsync(template, model, ct);
    }
}