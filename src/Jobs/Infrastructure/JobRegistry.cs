using Avolutions.Baf.Core.Jobs.Abstractions;

namespace Avolutions.Baf.Core.Jobs.Infrastructure;

public sealed class JobRegistry : IJobRegistry
{
    private readonly Dictionary<string,IJob> _byKey;
    public JobRegistry(IEnumerable<IJob> jobs) 
        => _byKey = jobs.ToDictionary(j => j.Key, StringComparer.OrdinalIgnoreCase);
    public IReadOnlyList<IJob> All => _byKey.Values.ToList();
    public IJob? Get(string key) => _byKey.GetValueOrDefault(key);
}