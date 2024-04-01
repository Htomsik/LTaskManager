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
    public ObservableCollection<TaskProcess> Processes { get; set; } = new ();
    
    [Reactive]
    public TaskProcess? CurrentProcess { get; set; }
    
    [Reactive]
    public bool ShowOnlySystemProcess { get; set; }
    
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
                .Subscribe(_ => SetSubscribes());

            this.WhenAnyValue(x => x._appSettingStore.CurrentValue.Agreement)
                .Subscribe(_ => SetSubscribes());
        };
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
            .Select(currentSeconds => UpdateTimerSeconds  - currentSeconds)
            .TakeWhile(currentSeconds => currentSeconds >= 0)
            .Subscribe(OnTimerChange);
    }
    
    
    /// <summary>
    ///     Установка таймера на обновление
    /// </summary>
    private void SetSubscribes()
    {
        _processDisposable?.Dispose();

        if (!_appSettingStore.CurrentValue.Agreement)
        {
            Processes.Clear();
            return;
        }
        
        // Нужен на случаи первых вызовов
        if (Processes.Count == 0)
        {
            UpdateProcesses();
        }
        
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
        
        this.Log().Warn($"Process {CurrentProcess?.ProcessName} was killed");
    }

    
    /// <summary>
    ///     Обновление текущего списка процессов
    /// </summary>
    public void UpdateProcesses()
    {
        var operationTimer = new Stopwatch();
        operationTimer.Start();
        
        this.Log().Info($"Start processing {nameof(ProcessService)}:{nameof(UpdateProcesses)}");
        
        try
        {
            // При очистке медленнее, пересоздание быстрее
            Processes = new ObservableCollection<TaskProcess>();
            foreach (var process in Process.GetProcesses())
            {
                var taskProcess = new TaskProcess(process);
                Processes.Add(taskProcess);
            }
            
            SetSubscribes();

            InvokeProcessSubscriptions();
        }
        catch (Exception e)
        {
            this.Log().Error($"Can't {nameof(UpdateProcesses)} {nameof(Processes)} . {e.Message}");
            return;
        }
        
        operationTimer.Stop();
        
        this.Log().Warn($"{nameof(UpdateProcesses)} {nameof(Processes)} sucess. Elapsed Time {operationTimer.ElapsedMilliseconds} ms");
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
