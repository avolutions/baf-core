using System.Globalization;
using Avolutions.Baf.Core.Import.Abstractions;
using Avolutions.Baf.Core.Import.Models;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Avolutions.Baf.Core.Import.Services;

public abstract class CsvImportService<T, TRow> : IFileImportService
{
    protected readonly DbContext DbContext;

    protected CsvImportService(DbContext db)
    {
        DbContext = db;
    }

    protected virtual CsvConfiguration Configuration => new(CultureInfo.InvariantCulture)
    {
        Delimiter = ";",
        HasHeaderRecord = true,
        IgnoreBlankLines = true,
        TrimOptions = TrimOptions.Trim
    };
    
    public virtual string Type => string.Empty;
    public virtual string Description => string.Empty;
    public string FileExtension => ".csv";

    public virtual async Task<ImportResult> ImportAsync(Stream stream, ExistingRecordHandling existingHandling, CancellationToken cancellationToken = default)
    {
        var result = new ImportResult();
        
        try
        {
            using var reader = new StreamReader(stream);
            using var parser = new CsvParser(reader, Configuration);
            using var csv = new CsvReader(parser);

            // Read header once so we can do a manual row loop and catch row-level errors.
            if (await csv.ReadAsync())
            {
                csv.ReadHeader();
            }

            while (await csv.ReadAsync())
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    var row = csv.GetRecord<TRow>();
                    
                    var existingRecord = await GetExistingRecordAsync(row, cancellationToken);
                    if (existingRecord != null)
                    {
                        if (existingHandling == ExistingRecordHandling.Ignore)
                        {
                            result.RecordsIgnored++;
                            continue;
                        }
                    
                        result.RecordsUpdated += await UpdateRecordAsync(existingRecord, row, cancellationToken);
                    }
                    else
                    {
                        result.RecordsCreated += await CreateRecordAsync(row, cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    var rowNumber = csv.Context?.Parser?.Row;
                    result.Errors.Add(new CsvImportError(ex.Message, rowNumber));
                }
            }
        }
        catch (Exception ex)
        {
            result.Errors.Add(new CsvImportError(ex.Message));
        }

        return result;
    }

    protected abstract Task<int> CreateRecordAsync(TRow row, CancellationToken cancellationToken);

    protected abstract Task<int> UpdateRecordAsync(T existingRecord, TRow row, CancellationToken cancellationToken);

    protected abstract Task<T?> GetExistingRecordAsync(TRow row, CancellationToken cancellationToken);
}