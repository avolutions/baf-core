using Avolutions.Baf.Core.Import.Abstractions;

namespace Avolutions.Baf.Core.Import.Models;

public class CsvImportError : IImportError
{
    public int Row { get; }
    public string Message { get; }
    
    public CsvImportError(string message, int? row = null) 
    {
        Row = row ?? -1;
        Message = message;
    }
}