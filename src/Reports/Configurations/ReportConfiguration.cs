using System.Reflection;
using Avolutions.Baf.Core.Reports.Abstractions;
using Avolutions.Baf.Core.Reports.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Avolutions.Baf.Core.Reports.Configurations;

public class ReportConfiguration : IEntityTypeConfiguration<Report>
{
    public void Configure(EntityTypeBuilder<Report> builder)
    {
        builder.HasIndex(a => a.Key)
            .IsUnique();

        builder.Property(a => a.Key)
            .HasMaxLength(128);
        
        builder.Property(x => x.Json)
            .HasColumnType("jsonb");
        
        var discriminator = builder.HasDiscriminator<string>(nameof(Report.Key));

        // Scan all loaded assemblies for concrete Report types
        foreach (var type in AppDomain.CurrentDomain.GetAssemblies()
                     .SelectMany(a => a.GetTypes())
                     .Where(t => typeof(Report).IsAssignableFrom(t) 
                                 && !t.IsAbstract 
                                 && typeof(IReportWithKey).IsAssignableFrom(t)))
        {
            var prop = type.GetProperty(nameof(IReportWithKey.ReportKey), BindingFlags.Public | BindingFlags.Static);
            if (prop != null && prop.PropertyType == typeof(string))
            {
                var key = (string?)prop.GetValue(null);
                if (!string.IsNullOrWhiteSpace(key))
                {
                    discriminator.HasValue(type, key);
                }
            }
        }
    }
}