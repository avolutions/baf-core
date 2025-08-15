using Avolutions.BAF.Core.NumberSequences.Models;
using Avolutions.BAF.Core.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Avolutions.BAF.Core.NumberSequences.Services;

public class NumberSequenceService : INumberSequenceService
{
    private readonly BafDbContext _context;
    private readonly DbSet<NumberSequence> _dbSet;

    public NumberSequenceService(BafDbContext context)
    {
        _context = context;
        _dbSet = context.Set<NumberSequence>();
    }

    public async Task<string> GetNextAsync(string name)
    {
        using var tx = await _context.Database.BeginTransactionAsync();

        var sequence = await _context.NumberSequences.FirstOrDefaultAsync(s => s.Name == name);
        if (sequence is null)
            throw new InvalidOperationException($"Number sequence '{name}' not found.");

        var number = sequence.NextNumber;
        sequence.NextNumber++;

        await _context.SaveChangesAsync();
        await tx.CommitAsync();

        return Format(sequence, number);
    }

    public string Format(NumberSequence sequence, long number)
    {
        return $"{sequence.Prefix}{number.ToString($"D{sequence.NumberLength}")}";
    }
    
    public async Task<List<NumberSequence>> GetAllAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task UpdateAsync(List<NumberSequence> sequences)
    {
        foreach (var sequence in sequences)
        {
            var tracked = _context.ChangeTracker
                .Entries<NumberSequence>()
                .FirstOrDefault(e => e.Entity.Name == sequence.Name);

            if (tracked is not null)
            {
                tracked.State = EntityState.Detached;
            }

            _context.Update(sequence);
        }

        await _context.SaveChangesAsync();
    }
}

public class NumberSequenceService<T> : INumberSequenceService<T>
    where T : INumberSequenceDefinition
{
    private readonly INumberSequenceService _inner;

    public NumberSequenceService(INumberSequenceService inner)
    {
        _inner = inner;
    }

    public Task<string> GetNextAsync()
    {
        return _inner.GetNextAsync(T.Name);
    }
}
