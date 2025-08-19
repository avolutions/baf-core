using System.ComponentModel.DataAnnotations;

namespace Avolutions.Baf.Core.NumberSequences.Models;

public class NumberSequence
{
    [Key]
    public string Name { get; set; } = string.Empty;
    public int NextNumber { get; set; } = 1;
    public int NumberLength { get; set; } = 6;
    public string Prefix { get; set; } = string.Empty;
}