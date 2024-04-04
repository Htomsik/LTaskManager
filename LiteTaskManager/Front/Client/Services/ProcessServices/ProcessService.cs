using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive.Linq;
using AppInfrastructure.Stores.DefaultStore;
using Client.Infrastructure.Logging;
using Client.Models;
using Client.Services.ComputerInfoService;
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
    
    public double UpdateTimerSeconds => _appSettingStore.CurrentValue.ProcessUpdateTimeOut;

    public double ReCalcTimerSeconds => _appSettingStore.CurrentValue.ProcessReCalcTimeOut;
    
    public event Action<double>? UpdateTimerChangeNotifier;

    public event Action? ProcessesChanged;
    
    #endregion

    #region Fiels
    
    private IDisposable _processDisposable;
    
    /// <summary>
    ///     Таймер перезагрузки процессов
    /// </summary>
    private IDisposable? _updateProcessTimer;
    
    /// <summary>
    ///     Таймер перерасчета данных процессов
    /// </summary>
    private IDisposable? _reCalcProcessTimer;
    
    /// <summary>
    ///     Производится ли обновелние процессов
    /// </summary>
    private bool _canReCalc = true;
    
    private readonly IStore<AppSettings> _appSettingStore;
    
    private readonly IComputerInfoService _computerInfoService;
    
    
    #endregion

    #region Constructions

    public ProcessService(IStore<AppSettings> appSettingStore, IComputerInfoService computerInfoService)
    {
        _appSettingStore = appSettingStore;
        _computerInfoService = computerInfoService;

        appSettingStore.CurrentValueChangedNotifier += () =>
        {
            this.WhenAnyValue(x => x._appSettingStore.CurrentValue.ProcessUpdateTimeOut)
                .Throttle(TimeSpan.FromSeconds(1))
                .Subscribe(_ => SetUpdateSubscribes());

            this.WhenAnyValue(x => x._appSettingStore.CurrentValue.Agreement)
                .Subscribe(_ => SetUpdateSubscribes());
        };
    }

    #endregion

    #region Methods

    /// <summary>
    ///     Вызов уведомления об изменении оставшегося времени до пересоздания процессов
    /// </summary>
    private void UpdateOnTimerChange(double currentSec)
    {
        UpdateTimerChangeNotifier?.Invoke(currentSec);

        if (currentSec != 0) return;
        
        UpdateProcesses();
    } 
    
    /// <summary>
    ///     Вызов уведомления об изменении оставшегося времени до пересоздания процессов
    /// </summary>
    private void ReCalcOnTimerChange(double currentSec)
    {
        if (currentSec != 0) return;
        
        ReCalcProcess();
        StartReCalcTimer();
    } 
    
    /// <summary>
    ///     Установки подписки для пересоздания процессов
    /// </summary>
    private void StartUpdateTimer()
    {
        _updateProcessTimer?.Dispose();
        
        _updateProcessTimer = Observable
            .Timer(DateTimeOffset.Now, TimeSpan.FromSeconds(1))
            .Select(currentSeconds => UpdateTimerSeconds  - currentSeconds)
            .TakeWhile(currentSeconds => currentSeconds >= 0)
            .Subscribe(UpdateOnTimerChange);

        StartReCalcTimer();
    }

    /// <summary>
    ///     Установки подписки для обновелния данных процессов
    /// </summary>
    private void StartReCalcTimer()
    {
        _reCalcProcessTimer?.Dispose();
        
        _reCalcProcessTimer = Observable
            .Timer(DateTimeOffset.Now, TimeSpan.FromSeconds(1))
            .Select(currentSeconds => ReCalcTimerSeconds  - currentSeconds)
            .TakeWhile(currentSeconds => currentSeconds >= 0)
            .Subscribe(ReCalcOnTimerChange);
    }
    
    
    /// <summary>
    ///     Установка таймера на обновление
    /// </summary>
    private void SetUpdateSubscribes()
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
            .Subscribe(_ => StartUpdateTimer());
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
            this.Log().StructLogError($"Process {CurrentProcess?.ProcessName} not killed", e.Message);
            return;
        }
        
        if (CurrentProcess is not null)
        {
            Processes.Remove(CurrentProcess);
        }
        
        this.Log().StructLogInfo($"Process {CurrentProcess?.ProcessName} was killed");
    }

    
    /// <summary>
    ///     Создание нового списка процессов
    /// </summary>
    public void UpdateProcesses()
    {
        _canReCalc = false;
        
        new Action(() =>
        {
            // При очистке медленнее, пересоздание быстрее
            Processes = new ObservableCollection<TaskProcess>();
            foreach (var process in Process.GetProcesses())
            {
                var taskProcess = new TaskProcess(process);
                Processes.Add(taskProcess);
            }
            
        }).TimeLog(this.Log());


        ReCalcProcess();
        
        // Обновление подписок
        SetUpdateSubscribes();
        InvokeProcessSubscriptions();

        _canReCalc = true;
    }
    
    /// <summary>
    ///     Обновление текущих процессов
    /// </summary>
    private void ReCalcProcess()
    {
        if (!_canReCalc)
        {
            return;
        }
        
        new Action(() =>
        {
            foreach (var process in Processes)
            {
                process.Refresh(_computerInfoService);
            }

            
        }).TimeLog(this.Log());
    }
    
    /// <summary>
    ///     Метод уведомления о необходимости перепривязки подписок
    /// </summary>
    private void InvokeProcessSubscriptions()
    {
        ProcessesChanged?.Invoke();

        if (ProcessesChanged is not null)
        {
            this.Log().StructLogDebug("Invoked");
        }
    }
    
    #endregion
}
