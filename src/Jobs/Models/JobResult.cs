namespace Avolutions.Baf.Core.Jobs.Models;

public sealed record JobResult(
    bool Success,
    string Message,
    object? Data = null
);