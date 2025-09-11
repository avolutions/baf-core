using Avolutions.Baf.Core.Import.Abstractions;

namespace Avolutions.Baf.Core.Import.Models;

public class ImportResult
{
    public bool Success => Errors.Count == 0;
    public List<IImportError> Errors { get; set; } = [];
    public int RecordsCreated { get; set; }
    public int RecordsUpdated { get; set; }
    public int RecordsIgnored { get; set; }
    public int RecordsImported => RecordsCreated + RecordsUpdated;
    public int RecordsProcessed => RecordsCreated + RecordsUpdated + RecordsIgnored;
}