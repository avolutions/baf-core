using System.Text.Json;
using System.Threading.Channels;
using Avolutions.Baf.Core.Jobs.Abstractions;
using Avolutions.Baf.Core.Jobs.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Avolutions.Baf.Core.Jobs.Infrastructure;

public sealed class JobWorker : BackgroundService
{
    private readonly ChannelReader<JobRequest> _reader;
    private readonly IJobRegistry _registry;
    private readonly IServiceScopeFactory _scopeFactory;

    public JobWorker(Channel<JobRequest> channel, IJobRegistry registry, IServiceScopeFactory scopeFactory)
    {
        _reader = channel.Reader;
        _registry = registry;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var req in _reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<BafDbContext>();

                var run = await db.JobRuns.FindAsync([req.RunId], stoppingToken);
                if (run is null)
                {
                    continue;
                }

                run.Status = JobRunStatus.Running;
                run.StartedAt = DateTimeOffset.UtcNow;
                await db.SaveChangesAsync(stoppingToken);

                var job = _registry.Get(req.JobKey) ?? throw new InvalidOperationException($"Unknown job '{req.JobKey}'.");
                var param = JsonSerializer.Deserialize(req.ParamJson, job.ParamType)!;

                var result = await job.ExecuteAsync(param, stoppingToken);

                run.Status = result.Success ? JobRunStatus.Succeeded : JobRunStatus.Failed;
                run.ResultJson = JsonSerializer.Serialize(result);
                run.FinishedAt = DateTimeOffset.UtcNow;
                await db.SaveChangesAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { /* shutdown */ }
            catch (Exception ex)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<BafDbContext>();
                    var run = await db.JobRuns.FindAsync([req.RunId], stoppingToken);
                    if (run is not null)
                    {
                        run.Status = JobRunStatus.Failed;
                        run.Error = ex.Message;
                        run.FinishedAt = DateTimeOffset.UtcNow;
                        await db.SaveChangesAsync(stoppingToken);
                    }
                }
                catch { /* best effort */ }
            }
        }
    }
}
