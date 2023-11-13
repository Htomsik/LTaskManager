using System.Diagnostics;
using System.Reactive;
using Client.Services.AppInfoService;
using ReactiveUI;

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
        
        OpenGitHubUrl = ReactiveCommand.Create(() =>
        {
            Process.Start(new ProcessStartInfo{FileName = AppInfoService.AppGitHub, UseShellExecute = true});
        });
    }
    public ReactiveCommand<Unit, Unit> OpenGitHubUrl { get; set; }
}