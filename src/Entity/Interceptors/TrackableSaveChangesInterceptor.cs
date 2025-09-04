using System.Security.Claims;
using Avolutions.Baf.Core.Entity.Abstractions;
using Avolutions.Baf.Core.Identity.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Avolutions.Baf.Core.Entity.Interceptors;

public sealed class TrackableSaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly IHttpContextAccessor _http;

    public TrackableSaveChangesInterceptor(IHttpContextAccessor http)
    {
        _http = http;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken ct = default)
    {
        var context = eventData.Context!;
        var now = DateTime.UtcNow;
        var userId = GetUserIdOrSystem();

        var entries = context.ChangeTracker.Entries()
            .Where(e => e is { Entity: ITrackable, State: EntityState.Added or EntityState.Modified });

        foreach (var entry in entries)
        {
            var entity = (ITrackable)entry.Entity;
            if (entry.State == EntityState.Added)
            {
                entity.CreatedAt = now;
                entity.CreatedBy = userId;
            }

            entity.ModifiedAt = now;
            entity.ModifiedBy = userId;
        }

        return base.SavingChangesAsync(eventData, result, ct);
    }

    private Guid GetUserIdOrSystem()
    {
        var userIdString = _http.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userIdString, out var parsedId) ? parsedId : SystemUser.Id;
    }
}