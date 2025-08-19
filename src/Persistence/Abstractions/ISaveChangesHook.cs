using Microsoft.EntityFrameworkCore;

namespace Avolutions.Baf.Core.Persistence.Abstractions;

public interface ISaveChangesHook
{
    int Order => 0;
    Task OnBeforeSaveChanges(DbContext context, CancellationToken cancellationToken) => Task.CompletedTask;
    Task OnAfterSaveChanges(DbContext context, int rows, CancellationToken cancellationToken) => Task.CompletedTask;
}