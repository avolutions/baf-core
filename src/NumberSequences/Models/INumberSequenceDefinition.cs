namespace Avolutions.Baf.Core.NumberSequences.Models;

public interface INumberSequenceDefinition
{
    static abstract string Name { get; }

    static abstract NumberSequence Defaults { get; }
}
