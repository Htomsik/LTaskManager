using Client.Services.AppInfoService.Base;
using Client.Services.Base;
using ReactiveUI.Fody.Helpers;

namespace Client.ViewModels;

/// <summary>
///     Вьюмодель статус бара, отображаюшего важную информацию
/// </summary>
internal sealed class StatusBarViewModel : ViewModelBase
{
    /// <summary>
    ///     Сервис информации о приложении
    /// </summary>
    [Reactive]
    public IAppInfoService AppInfoService { get; set; }

    /// <summary>
    ///     Максмальное время обновления
    /// <remarks> Требуется для задачи порогового значения в процесс баре</remarks>
    /// </summary>
    [Reactive]
    public double ProcessesMaxUpdateTime { get; set; }
    
    /// <summary>
    ///     Оставшнееся время до обновленния
    /// <remarks> Требуется для задачи текущего значения в процесс баре</remarks>
    /// </summary>
    [Reactive]
    public double ProcessesCurrentUpdateTime{ get; set; }
    
    /// <param name="processService"> Сервис обработки процессов </param>
    /// <param name="appInfoService"> Сервис предоставляющий информацию о прилоджении </param>
    public StatusBarViewModel(IProcessService processService, IAppInfoService appInfoService)
    {
        AppInfoService = appInfoService;
        
        // Подписка на изменение таймера одо обновления процессов
        processService.UpdateTimer.TimerChangedNotifier += sec =>
        {
            ProcessesCurrentUpdateTime = processService.UpdateTimer.UpdateDelaySeconds - sec;
            ProcessesMaxUpdateTime = processService.UpdateTimer.UpdateDelaySeconds;
        };
        
        ProcessesMaxUpdateTime = processService.UpdateTimer.UpdateDelaySeconds;
    }
}