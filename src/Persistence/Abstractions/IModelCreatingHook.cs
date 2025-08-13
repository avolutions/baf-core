using Microsoft.EntityFrameworkCore;

namespace Avolutions.BAF.Core.Persistence.Abstractions;

public interface IModelCreatingHook
{
    int Order => 0;

    /// <summary>
    /// Apply custom model configuration. Called inside OnModelCreating.
    /// </summary>
    void Configure(ModelBuilder modelBuilder);
}