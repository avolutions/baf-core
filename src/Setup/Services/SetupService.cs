using Avolutions.Baf.Core.Setup.Infrastructure;
using Avolutions.Baf.Core.Setup.Models;
using Microsoft.EntityFrameworkCore;

namespace Avolutions.Baf.Core.Setup.Services;

public class SetupService(BafDbContext db, SetupState state) : ISetupService
{
    public Task<bool> RequiresSetupAsync(CancellationToken ct = default)
    {
        return  Task.FromResult(!state.IsCompleted);
    }

    public async Task CompleteSetupAsync(CancellationToken ct = default)
    {
        // If there is no row yet, insert one; otherwise update it.
        var row = await db.SetupStatus.SingleOrDefaultAsync(ct);
        if (row is null)
        {
            db.SetupStatus.Add(new SetupStatus { Id = 1, IsCompleted = true });
        }
        else if (!row.IsCompleted)
        {
            row.IsCompleted = true;
        }

        await db.SaveChangesAsync(ct);

        // Flip in-memory flag so middleware stops redirecting immediately
        state.IsCompleted = true;
    }

    public async Task InitializeAsync(CancellationToken ct = default)
    {
        // Empty table => not installed
        var completed = await db.SetupStatus
            .AsNoTracking()
            .AnyAsync(s => s.IsCompleted, ct);

        state.IsCompleted = completed;
    }
}