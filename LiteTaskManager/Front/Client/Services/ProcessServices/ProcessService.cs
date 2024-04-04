using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive.Linq;
using AppInfrastructure.Stores.DefaultStore;
using Client.Infrastructure.Logging;
using Client.Models;
using Client.Services.ComputerInfoService;
using Client.Timers.Base;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace Client.Services;

internal sealed class ProcessService : ReactiveObject, IProcessService<TaskProcess>
{
    #region Properties
    public IReactiveTimer UpdateTimer { get; }
    
    public IReactiveTimer RefreshTimer { get;  }
    
    [Reactive]
    public ObservableCollection<TaskProcess> Processes { get; set; } = new ();
    
    [Reactive]
    public TaskProcess? CurrentProcess { get; set; }
    
    public event Action? ProcessesChanged;
    
    #endregion

    #region Fiels
    
    private IDisposable _processDisposable;
    
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
                .Subscribe(_ =>
                {
                    StartTimers();
                });
            
            this.WhenAnyValue(x => x._appSettingStore.CurrentValue.ProcessReCalcTimeOut)
                .Throttle(TimeSpan.FromSeconds(1))
                .Subscribe(_ =>
                {
                    if (RefreshTimer != null)
                        RefreshTimer.UpdateDelaySeconds = _appSettingStore.CurrentValue.ProcessReCalcTimeOut;
                });

            this.WhenAnyValue(x => x._appSettingStore.CurrentValue.Agreement)
                .Subscribe(_ => SetUpdateSubscribes());
        };


        UpdateTimer = new ReactiveTimer(UpdateProcesses)
        {
            UpdateDelaySeconds = appSettingStore.CurrentValue.ProcessUpdateTimeOut
        };

        RefreshTimer = new ReactiveTimer(RefreshProcess)
        {
            UpdateDelaySeconds = appSettingStore.CurrentValue.ProcessReCalcTimeOut
        };
    }

    #endregion

    #region Methods
    
    /// <summary>
    ///     Установка таймера на обновление
    /// </summary>
    private void SetUpdateSubscribes()
    {
        _processDisposable?.Dispose();

        if (!_appSettingStore.CurrentValue.Agreement)
        {
            Processes?.Clear();
            return;
        }
        
        // Нужен на случаи первых вызовов
        if (Processes.Count == 0)
        {
            UpdateProcesses();
        }
        
        _processDisposable = this.WhenAnyValue(x => x.Processes)
             .Subscribe(_ => StartTimers());
    }

    private void StartTimers()
    {
        UpdateTimer.UpdateDelaySeconds = _appSettingStore.CurrentValue.ProcessUpdateTimeOut;
        RefreshTimer.UpdateDelaySeconds = _appSettingStore.CurrentValue.ProcessReCalcTimeOut;
        
        UpdateTimer.Start();
        RefreshTimer.Start();
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


        RefreshProcess();
        
        // Обновление подписок
        SetUpdateSubscribes();
        InvokeProcessSubscriptions();

        _canReCalc = true;
    }

    public void RefreshProcess()
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
