using Avolutions.Baf.Core.Module.Abstractions;
using Avolutions.Baf.Core.Reports.Abstractions;
using Avolutions.Baf.Core.Reports.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Avolutions.Baf.Core.Reports;

public class ReportsModule : IFeatureModule
{
    public void Register(IServiceCollection services)
    {
        services.AddScoped<IReportService, ReportService>();
    }
}