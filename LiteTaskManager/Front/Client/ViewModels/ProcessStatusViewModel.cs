using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AppInfrastructure.Stores.DefaultStore;
using Client.Infrastructure.Logging;
using Client.Models;
using Client.Services.Base;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;


namespace Client.ViewModels;

/// <summary>
///     Вьюмодель статуса обновления процессов
/// </summary>
internal sealed class ProcessStatusViewModel : ViewModelBase
{
    /// <summary>
    ///     Максмальное время обновления
    /// <remarks> Требуется для задачи порогового значения в процесс баре</remarks>
    /// </summary>
    [Reactive]
    public double ProcessesMaxUpdateTime { get; set; }
    
    /// <summary>
    ///     Настройки приложения
    /// </summary>
    [Reactive]
    public AppSettings Settings { get; set; }
    
    /// <summary>
    ///     Оставшнееся время до обновленния
    /// <remarks> Требуется для задачи текущего значения в процесс баре</remarks>
    /// </summary>
    [Reactive]
    public double ProcessesCurrentUpdateTime{ get; set; }
    
    #region Constructors

    public ProcessStatusViewModel(IProcessService processService,IStore<AppSettings> appSettings)
    {
        Settings = appSettings.CurrentValue;
        ProcessesMaxUpdateTime = processService.UpdateTimer.UpdateDelaySeconds;
        
        #region Commands

        UpdateProcesses = ReactiveCommand.CreateFromObservable(
            () =>
                Observable
                    .StartAsync(ct => Task.Run(()=>processService.UpdateProcesses(false, false), ct)));

        RefreshProcesses = ReactiveCommand.CreateFromObservable(
            () =>
                Observable
                    .StartAsync(ct => Task.Run(processService.RefreshProcess, ct)), UpdateProcesses.IsExecuting.Select(x=>!x));

        #endregion

        #region Commands logging

        UpdateProcesses.ThrownExceptions.Subscribe(e =>
            this.Log().StructLogError($"Processing", e.Message, nameof(UpdateProcesses)));
        
        UpdateProcesses.Subscribe(_ => this.Log().StructLogInfo($"Processing", nameof(UpdateProcesses)));
      
        RefreshProcesses.ThrownExceptions.Subscribe(e =>
            this.Log().StructLogError($"Processing", e.Message, nameof(RefreshProcesses)));
        
        RefreshProcesses.Subscribe(_ => this.Log().StructLogInfo($"Processing", nameof(RefreshProcesses)));

        #endregion
        
        // Подписка на изменение таймера одо обновления процессов
        processService.UpdateTimer.TimerChangedNotifier += sec =>
        {
            ProcessesCurrentUpdateTime = processService.UpdateTimer.UpdateDelaySeconds - sec;
            ProcessesMaxUpdateTime = processService.UpdateTimer.UpdateDelaySeconds;
        };

        appSettings.CurrentValueChangedNotifier += () =>
        {
            Settings = appSettings.CurrentValue;
        };
    }

    #endregion

    #region Commands

    /// <summary>
    ///   Обновление процессов
    /// </summary>
    public ReactiveCommand<Unit, Unit> UpdateProcesses { get; init; }
   
    /// <summary>
    ///   Перерасчет процессов
    /// </summary>
    public ReactiveCommand<Unit, Unit> RefreshProcesses { get; init; }

    #endregion
    
}