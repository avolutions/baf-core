using Avolutions.Baf.Core.Import.Models;

namespace Avolutions.Baf.Core.Import.Abstractions;

public interface IFileImportService
{
    string Type { get; }

    string Description { get; }
    
    string FileExtension { get; }

    Task<ImportResult> ImportAsync(Stream stream, ExistingRecordHandling existingHandling, CancellationToken cancellationToken = default);
}