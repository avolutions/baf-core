using Avolutions.Baf.Core.Loading.Abstractions;

namespace Avolutions.Baf.Core.Loading.Services;

public class LoadingService : ILoadingService
{
    private bool _isLoading;
    
    public bool IsLoading => _isLoading;
    
    public event Action? OnLoadingChanged;
    
    public virtual void StartLoading()
    {
        if (_isLoading)
        {
            return;
        }
        
        _isLoading = true;
        OnLoadingChanged?.Invoke();
    }
    
    public virtual void StopLoading()
    {
        if (!_isLoading)
        {
            return;
        }
        
        _isLoading = false;
        OnLoadingChanged?.Invoke();
    }
    
    protected void NotifyStateChanged()
    {
        OnLoadingChanged?.Invoke();
    }
}
