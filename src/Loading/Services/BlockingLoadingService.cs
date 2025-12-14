using Avolutions.Baf.Core.Loading.Abstractions;

namespace Avolutions.Baf.Core.Loading.Services;

public class BlockingLoadingService : LoadingService, IBlockingLoadingService
{
    private string _loadingText = string.Empty;
    public string LoadingText => _loadingText;
    
    public void StartLoading(string text)
    {
        _loadingText = text;
        base.StartLoading();
    }
    
    public void UpdateText(string text)
    {
        if (!IsLoading)
        {
            return;
        }
        
        _loadingText = text;
        NotifyStateChanged();
    }
    
    public override void StopLoading()
    {
        base.StopLoading();
        _loadingText = string.Empty;
    }
}
