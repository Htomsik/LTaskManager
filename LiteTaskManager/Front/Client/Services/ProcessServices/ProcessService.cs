using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive.Linq;
using AppInfrastructure.Stores.DefaultStore;
using Client.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace Client.Services;

internal sealed class ProcessService : ReactiveObject, IProcessService<Process>
{
    #region Properties

    [Reactive]
    public ObservableCollection<Process> Processes { get; set; }
    
    [Reactive]
    public Process? CurrentProcess { get; set; }

    public double UpdateTimerSeconds => _appSettingStore.CurrentValue.ProcessUpdateTimeOut;

    #endregion

    #region Fiels
    
    private IDisposable _processDisposable;
    
    private IDisposable? _timer;
    
    private readonly IStore<AppSettings> _appSettingStore;
    
    public event Action<double>? UpdateTimerChangeNotifier;
    
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
            .Select(currentSeconds => UpdateTimerSeconds - currentSeconds)
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
            Processes = new ObservableCollection<Process>(Process.GetProcesses());

            SetSubscribes();
        }
        catch (Exception e)
        {
            this.Log().Error($"Can't {nameof(UpdateProcesses)} {nameof(Processes)} . {e.Message}");
            return;
        }
        
        this.Log().Warn($"{nameof(UpdateProcesses)} {nameof(Processes)} sucess");
    }
    
    #endregion
}
