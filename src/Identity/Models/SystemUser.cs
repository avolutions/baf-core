namespace Avolutions.Baf.Core.Identity.Models;

/// <summary>
/// Special technical account used internally for CreatedBy/ModifiedBy defaults.
/// Cannot log in and has no password.
/// </summary>
public static class SystemUser
{
    public static readonly Guid Id = Guid.Parse("4bd320c0-8ff1-429e-a2f9-913f9db556b9");
    public const string Lastname = "System";
    public const string UserName = "system";
    public const string NormalizedUserName = "SYSTEM";
    public const string Email = "system@local";
    public const string NormalizedEmail = "SYSTEM@LOCAL";
    public const string SecurityStamp = "6LKO53DYVZ6QK7YR6L4KFXDOAZTGIQWI";
    public const string ConcurrencyStamp = "2830f4b6-280f-478d-a7b5-3b945bac319b";
}