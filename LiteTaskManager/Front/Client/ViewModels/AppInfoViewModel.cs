using Client.Services.AppInfoService;

namespace Client.ViewModels;

/// <summary>
///     VMD информации о приложении
/// </summary>
internal sealed class AppInfoViewModel : ViewModelBase
{
    /// <summary>
    ///     Сервис информации о приложении
    /// </summary>
    public IAppInfoService AppInfoService { get; }
    
    public AppInfoViewModel(IAppInfoService appInfoService)
    {
        AppInfoService = appInfoService;
    }
}