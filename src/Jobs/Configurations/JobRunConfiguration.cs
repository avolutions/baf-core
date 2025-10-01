using Avolutions.Baf.Core.Identity.Models;
using Avolutions.Baf.Core.Jobs.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Avolutions.Baf.Core.Jobs.Configurations;

public class JobRunConfiguration : IEntityTypeConfiguration<JobRun>
{
    public void Configure(EntityTypeBuilder<JobRun> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.JobKey)
            .HasMaxLength(200)
            .IsRequired();
        
        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(40);
        
        builder.Property(x => x.ParamJson)
            .IsRequired();
        
        builder.HasIndex(x => x.JobKey);
        
        builder.HasIndex(x => x.QueuedAt);
        
        builder.HasIndex(x => x.TriggeredBy);
        
        builder.Property(x => x.TriggeredBy)
            .HasDefaultValue(SystemUser.Id);
    }
}