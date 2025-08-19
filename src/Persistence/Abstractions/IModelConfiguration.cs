using Microsoft.EntityFrameworkCore;

namespace Avolutions.Baf.Core.Persistence.Abstractions;

public interface IModelConfiguration
{
    void Configure(ModelBuilder modelBuilder);
}