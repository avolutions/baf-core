namespace Avolutions.Baf.Core.Setup.Services;

public interface ISetupService
{
    Task<bool> RequiresSetupAsync(CancellationToken ct = default);
    Task CompleteSetupAsync(CancellationToken ct = default);
    Task InitializeAsync(CancellationToken ct = default);
}