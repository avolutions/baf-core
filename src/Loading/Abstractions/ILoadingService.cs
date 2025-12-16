namespace Avolutions.Baf.Core.Loading.Abstractions;

public interface ILoadingService
{
    bool IsLoading { get; }
    event Action? OnLoadingChanged;
    void StartLoading();
    void StopLoading();
}