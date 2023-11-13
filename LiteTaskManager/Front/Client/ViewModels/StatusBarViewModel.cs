using Client.Models;
using Client.Services;
using Client.Services.AppInfoService;
using ReactiveUI.Fody.Helpers;

namespace Client.ViewModels;

internal sealed class StatusBarViewModel : ViewModelBase
{
    [Reactive]
    public IAppInfoService AppInfoService { get; set; }

    [Reactive]
    public double ProcessesMaxUpdateTime { get; set; }
    
    [Reactive]
    public double ProcessesCurrentUpdateTime{ get; set; }
    
    public StatusBarViewModel(IProcessService<TaskProcess> processService, IAppInfoService appInfoService)
    {
        AppInfoService = appInfoService;
        
        processService.UpdateTimerChangeNotifier += sec =>
        {
            ProcessesCurrentUpdateTime = processService.UpdateTimerSeconds - sec;
            ProcessesMaxUpdateTime = processService.UpdateTimerSeconds;
        };
        
        ProcessesMaxUpdateTime = processService.UpdateTimerSeconds;
    }
}