using Microsoft.EntityFrameworkCore;

namespace Avolutions.BAF.Core.Persistence.Abstractions;

public interface ISaveChangesHook
{
    int Order => 0;
    Task OnSavingAsync(DbContext context, CancellationToken ct) => Task.CompletedTask;
    Task OnSavedAsync(DbContext context, int rows, CancellationToken ct) => Task.CompletedTask;
}