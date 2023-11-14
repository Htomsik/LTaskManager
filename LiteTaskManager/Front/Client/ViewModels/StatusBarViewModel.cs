using Client.Models;
using Client.Services;
using Client.Services.AppInfoService;
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
    public StatusBarViewModel(IProcessService<TaskProcess> processService, IAppInfoService appInfoService)
    {
        AppInfoService = appInfoService;
        
        // Подписка на изменение таймера одо обновления процессов
        processService.UpdateTimerChangeNotifier += sec =>
        {
            ProcessesCurrentUpdateTime = processService.UpdateTimerSeconds - sec;
            ProcessesMaxUpdateTime = processService.UpdateTimerSeconds;
        };
        
        ProcessesMaxUpdateTime = processService.UpdateTimerSeconds;
    }
}