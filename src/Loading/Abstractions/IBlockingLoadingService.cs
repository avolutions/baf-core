namespace Avolutions.Baf.Core.Loading.Abstractions;

public interface IBlockingLoadingService : ILoadingService
{
    string LoadingText { get; }
    void StartLoading(string text);
    void UpdateText(string text);
}