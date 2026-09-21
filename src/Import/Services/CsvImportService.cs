using System.Globalization;
using System.Text;
using Avolutions.Baf.Core.Entity.Exceptions;
using Avolutions.Baf.Core.Import.Abstractions;
using Avolutions.Baf.Core.Import.Models;
using CsvHelper;
using CsvHelper.Configuration;

namespace Avolutions.Baf.Core.Import.Services;

public abstract class CsvImportService<T, TRow> : IFileImportService
{
    protected virtual CsvConfiguration Configuration => new(CultureInfo.InvariantCulture)
    {
        Delimiter = ";",
        HasHeaderRecord = true,
        IgnoreBlankLines = true,
        TrimOptions = TrimOptions.Trim
    };
    protected virtual Encoding FileEncoding => Encoding.UTF8;
    
    public virtual string Type => string.Empty;
    public virtual string Description => string.Empty;
    public string FileExtension => ".csv";

    public virtual async Task<ImportResult> ImportAsync(Stream stream, ExistingRecordHandling existingHandling, CancellationToken cancellationToken = default)
    {
        var result = new ImportResult();
        
        try
        {
            using var reader = new StreamReader(stream, FileEncoding);
            using var parser = new CsvParser(reader, Configuration);
            using var csv = new CsvReader(parser);

            // Read header once so we can do a manual row loop and catch row-level errors.
            if (await csv.ReadAsync())
            {
                csv.ReadHeader();
            }
            
            await OnImportStartingAsync(cancellationToken);

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
                catch (EntityValidationException ex)
                {
                    var rowNumber = csv.Context?.Parser?.Row;
                    foreach (var failure in ex.Failures)
                    {
                        result.Errors.Add(new CsvImportError(failure.ErrorMessage, rowNumber));
                    }
                }
                catch (Exception ex)
                {
                    var rowNumber = csv.Context?.Parser?.Row;
                    result.Errors.Add(new CsvImportError(ex.Message, rowNumber));
                }
            }
            
            try
            {
                await OnImportCompletedAsync(result, cancellationToken);
            }
            catch (Exception ex)
            {
                result.Errors.Add(new CsvImportError(ex.Message));
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
    protected virtual Task OnImportStartingAsync(CancellationToken ct)
    {
        return Task.CompletedTask;
    }
    protected virtual Task OnImportCompletedAsync(ImportResult result, CancellationToken ct)
    {
        return Task.CompletedTask;
    }
}