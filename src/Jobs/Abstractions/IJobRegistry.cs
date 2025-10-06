namespace Avolutions.Baf.Core.Jobs.Abstractions;

public interface IJobRegistry
{
    IReadOnlyList<IJob> All { get; }
    IJob? Get(string key);
}