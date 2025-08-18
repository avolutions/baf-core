using Microsoft.EntityFrameworkCore;

namespace Avolutions.BAF.Core.Persistence.Abstractions;

public interface IModelConfiguration
{
    void Configure(ModelBuilder modelBuilder);
}