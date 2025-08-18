namespace Avolutions.Baf.Core.Loading.Services;

public class LoadingService
{
    private bool _isLoading;
    
    public bool IsLoading => _isLoading;
    
    public event Action? OnLoadingChanged;
    
    public void StartLoading()
    {
        if (_isLoading) return;
        
        _isLoading = true;
        OnLoadingChanged?.Invoke();
    }
    
    public void StopLoading()
    {
        if (!_isLoading) return;
        
        _isLoading = false;
        OnLoadingChanged?.Invoke();
    }
}