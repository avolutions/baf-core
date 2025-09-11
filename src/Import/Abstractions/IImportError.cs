namespace Avolutions.Baf.Core.Import.Abstractions;

public interface IImportError
{
    public string Message { get; }
    public int Row { get; }
}