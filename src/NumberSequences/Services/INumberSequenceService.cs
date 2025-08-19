using Avolutions.Baf.Core.NumberSequences.Models;

namespace Avolutions.Baf.Core.NumberSequences.Services;

public interface INumberSequenceService<T> where T : INumberSequenceDefinition
{
    Task<string> GetNextAsync();
}

public interface INumberSequenceService
{
    Task<string> GetNextAsync(string name);
    string Format(NumberSequence sequence, long number);
    Task<List<NumberSequence>> GetAllAsync();
    Task UpdateAsync(List<NumberSequence> sequences);
}