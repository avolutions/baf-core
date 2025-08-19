using System.ComponentModel.DataAnnotations;

namespace Avolutions.Baf.Core.Setup.Models;

public class SetupStatus
{
    [Key]
    public int Id { get; set; } = 1;
    public bool IsCompleted { get; set; }
}