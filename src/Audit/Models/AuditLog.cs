using System.Text.Json.Nodes;
using Avolutions.Baf.Core.Entity.Models;

namespace Avolutions.Baf.Core.Audit.Models;

public class AuditLog : EntityBase
{
    public string Entity { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public AuditAction Action { get; set; }

    public string? OldState { get; set; }
    public string? NewState { get; set; }
}