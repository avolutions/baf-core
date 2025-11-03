using Avolutions.Baf.Core.Entity.Services;
using Avolutions.Baf.Core.Reports.Abstractions;
using Avolutions.Baf.Core.Reports.Models;
using Avolutions.Baf.Core.Template.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Playwright;
using NJsonSchema;
using NJsonSchema.Generation;

namespace Avolutions.Baf.Core.Reports.Services;

public class ReportService : EntityService<Report>
{
    private readonly ITemplateService _templateService;
    
    public ReportService(DbContext context, ITemplateService templateService) : base(context)
    {
        _templateService = templateService;
    }
    
    public async Task<Report> GetByKeyAsync(string key, CancellationToken ct = default)
    {
        var report = await DbSet.FirstOrDefaultAsync(t => t.Key == key, ct);
        if (report == null)
        {
            throw new InvalidOperationException($"Report with key '{key}' not found.");
        }
        
        return report;
    }
    
    public async Task<string> GetModelSchemaJsonAsync(string key, CancellationToken ct = default)
    {
        var report = await GetByKeyAsync(key, ct);
        var settings = new SystemTextJsonSchemaGeneratorSettings
        {
            FlattenInheritanceHierarchy = true
        };
        var schema = JsonSchema.FromType(report.ModelType, settings);
        
        return schema.ToJson();
    }
    
    public async Task<IReportModel> BuildModelAsync(Report report, IReportArgs? args, CancellationToken ct = default)
    {
        return await report.BuildModelAsync(Context, args, ct);
    }

    public async Task<IReportModel> BuildDemoAsync(Report report, CancellationToken ct = default)
    {
        return await report.BuildDemoAsync(Context, ct);
    }
    
    public async Task<byte[]> RenderPdfAsync(string reportKey, IReportModel model, CancellationToken ct = default)
    {
        var report = await GetByKeyAsync(reportKey, ct);
        return await RenderPdfAsync(report, model, ct);
    }

    public async Task<byte[]> RenderPdfAsync(string reportKey, IReportArgs? args, CancellationToken ct = default)
    {
        var report = await GetByKeyAsync(reportKey, ct);
        var model = await BuildModelAsync(report, args, ct);
        
        return await RenderPdfAsync(report, model, ct);
    }

    public async Task<byte[]> RenderPdfAsync(Report report, IReportModel model, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        var contentHtml = await _templateService.RenderTemplateAsync(report.ContentHtml, model, ct);
        var headerHtml  = await _templateService.RenderTemplateAsync(report.HeaderHtml, model, ct);
        var footerHtml  = await _templateService.RenderTemplateAsync(report.FooterHtml, model, ct);
        
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
            FooterTemplate = footerHtml
        });

        return pdf;
    }
}