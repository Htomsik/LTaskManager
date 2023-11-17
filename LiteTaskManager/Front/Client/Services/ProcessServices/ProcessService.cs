using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive.Linq;
using AppInfrastructure.Stores.DefaultStore;
using Client.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace Client.Services;

internal sealed class ProcessService : ReactiveObject, IProcessService<TaskProcess>
{
    #region Properties

    [Reactive]
    public ObservableCollection<TaskProcess> Processes { get; set; }
    
    [Reactive]
    public TaskProcess? CurrentProcess { get; set; }

    public double UpdateTimerSeconds => _appSettingStore.CurrentValue.ProcessUpdateTimeOut;

    #endregion

    #region Fiels
    
    private IDisposable _processDisposable;
    
    private IDisposable? _timer;
    
    private readonly IStore<AppSettings> _appSettingStore;
    
    public event Action<double>? UpdateTimerChangeNotifier;

    public event Action? ProcessSubscriptionsChanged;
    
    #endregion

    #region Constructions

    public ProcessService(IStore<AppSettings> appSettingStore)
    {
        _appSettingStore = appSettingStore;

        appSettingStore.CurrentValueChangedNotifier += () =>
        {
            this.WhenAnyValue(x => x._appSettingStore.CurrentValue.ProcessUpdateTimeOut)
                .Throttle(TimeSpan.FromSeconds(1))
                .Subscribe(x => SetSubscribes());
        };
        
        UpdateProcesses();
    }

    #endregion

    #region Methods

    /// <summary>
    ///     Вызов уведомления об изменении оставшегося
    /// </summary>
    private void OnTimerChange(double currentSec)
    {
        UpdateTimerChangeNotifier?.Invoke(currentSec);

        if (currentSec == 0)
        {
            UpdateProcesses();
        }
    } 
    
    /// <summary>
    ///     Установка уведомлятора о том через сколько секунд обновится список процессов
    /// </summary>
    private void StartTimer()
    {
        _timer?.Dispose();
        
        _timer = Observable
            .Timer(DateTimeOffset.Now, TimeSpan.FromSeconds(1))
            .Select(currentSeconds => UpdateTimerSeconds > 4 ? UpdateTimerSeconds : 4 - currentSeconds)
            .TakeWhile(currentSeconds => currentSeconds >= 0)
            .Subscribe(OnTimerChange);
    }
    
    
    /// <summary>
    ///     Установка таймера на обновление
    /// </summary>
    private void SetSubscribes()
    {
        _processDisposable?.Dispose();

        _processDisposable = this.WhenAnyValue(x => x.Processes)
            .Subscribe(_ => StartTimer());
    }
    
    /// <summary>
    ///     Остановка выбранного процесса
    /// </summary>
    public void StopCurrentProcess()
    {
        try
        {
            CurrentProcess?.Kill();
        }
        catch (Exception e)
        {
            this.Log().Error($"Can't {nameof(StopCurrentProcess)} {nameof(CurrentProcess)}. {e.Message}");
            return;
        }
        
        if (CurrentProcess is not null)
        {
            Processes.Remove(CurrentProcess);
        }
        
        this.Log().Warn($"Process {CurrentProcess.ProcessName} was killed");
    }

    
    /// <summary>
    ///     Обновление текущего списка процессов
    /// </summary>
    public void UpdateProcesses()
    {
        try
        {
            List<TaskProcess> ProcessList = new List<TaskProcess>() { };
            
            //ToDo Подумать над оптимизацией
            
            foreach (var Process in Process.GetProcesses())
            {
                TaskProcess taskProcessCopy = new TaskProcess(Process);
                ProcessList.Add(taskProcessCopy); 
            }
            
            Processes = new ObservableCollection<TaskProcess>(ProcessList);
            
            SetSubscribes();

            InvokeProcessSubscriptions();
        }
        catch (Exception e)
        {
            this.Log().Error($"Can't {nameof(UpdateProcesses)} {nameof(Processes)} . {e.Message}");
            return;
        }
        this.Log().Warn($"{nameof(UpdateProcesses)} {nameof(Processes)} sucess");
    }

    /// <summary>
    ///     Метод уведомления о необходимости перепривязки подписок
    /// </summary>
    private void InvokeProcessSubscriptions()
    {
        ProcessSubscriptionsChanged?.Invoke();

        if (ProcessSubscriptionsChanged is not null)
        {
            this.Log().Info($"{nameof(ProcessSubscriptionsChanged)} invoked");
        }
    }
    
    #endregion
}
