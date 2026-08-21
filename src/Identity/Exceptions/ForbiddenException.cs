namespace Avolutions.Baf.Core.Identity.Exceptions;

public sealed class ForbiddenException : Exception
{
    public ForbiddenException(string policy)
        : base($"Access denied. Required policy: '{policy}'.")
    {
        Policy = policy;
    }

    public string Policy { get; }
}