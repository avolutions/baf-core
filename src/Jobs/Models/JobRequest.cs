namespace Avolutions.Baf.Core.Jobs.Models;

public sealed record JobRequest(
    Guid RunId,
    string JobKey,
    string ParamJson
);