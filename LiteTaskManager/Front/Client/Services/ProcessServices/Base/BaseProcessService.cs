using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AppInfrastructure.Stores.DefaultStore;
using Client.Infrastructure.Logging;
using Client.Models;
using Client.Models.TaskProcess.Base;
using Client.Services.ComputerInfoService.Base;
using Client.Timers.Base;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace Client.Services.Base;

internal abstract class BaseProcessService<TProcess> : ReactiveObject, IProcessService where TProcess : IProcess
{
    #region Properties
    public IReactiveTimer UpdateTimer { get; protected set; }
    
    public IReactiveTimer RefreshTimer { get; protected set;  }

    [Reactive] public ObservableCollection<IProcess> Processes { get; protected set; }
    
    [Reactive] public IProcess CurrentProcess { get; set; }

    public event Action? ProcessesChanged;
    
    #endregion

    #region Fiels
    
    protected IDisposable? ProcessDisposable;
    
    /// <summary>
    ///     Возможно ли обновить процесс
    /// </summary>
    protected  bool CanReCalc = true;
    
    protected readonly IStore<AppSettings> AppSettingStore;
    
    protected readonly IComputerInfoService ComputerInfoService;
    
    #endregion

    #region Constructions

    protected BaseProcessService(IStore<AppSettings> appSettingStore, IComputerInfoService computerInfoService)
    {
        AppSettingStore = appSettingStore;
        ComputerInfoService = computerInfoService;

        Processes = new ObservableCollectionExtended<IProcess>();
        CurrentProcess = null!;
        
        appSettingStore.CurrentValueChangedNotifier += () =>
        {
            this.WhenAnyValue(x => x.AppSettingStore.CurrentValue.ProcessUpdateTimeOut)
                .Throttle(TimeSpan.FromSeconds(1))
                .Subscribe(_ =>
                {
                    StartTimers();
                });
            
            this.WhenAnyValue(x => x.AppSettingStore.CurrentValue.ProcessReCalcTimeOut)
                .Throttle(TimeSpan.FromSeconds(1))
                .Subscribe(_ =>
                {
                    if (RefreshTimer != null)
                        RefreshTimer.UpdateDelaySeconds = AppSettingStore.CurrentValue.ProcessReCalcTimeOut;
                });
            
            this.WhenAnyValue(x => x.AppSettingStore.CurrentValue.Agreement)
                .Subscribe(_ => SetUpdateSubscribes());

            this.WhenAnyValue(x => x.AppSettingStore.CurrentValue.ManualMode)
                .Subscribe(isManual =>
                {
                    if (isManual)
                    {
                        StopTimers();
                    }
                    else
                    {
                        StartTimers();
                    }
                });
        };


        UpdateTimer = new ReactiveTimer(()=> UpdateProcesses())
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
    private async void SetUpdateSubscribes()
    {
        ProcessDisposable?.Dispose();

        if (!AppSettingStore.CurrentValue.Agreement)
        {
            Processes.Clear();
            return;
        }
        
        // Нужен на случаи первых вызовов
        if (Processes.Count == 0)
        {
            await Task.Run(()=>UpdateProcesses());
        }
        
        ProcessDisposable = this.WhenAnyValue(x => x.Processes)
             .Subscribe(_ => StartTimers());
    }

    protected void StartTimers()
    {
        StopTimers();
        
        UpdateTimer.UpdateDelaySeconds = AppSettingStore.CurrentValue.ProcessUpdateTimeOut;
        RefreshTimer.UpdateDelaySeconds = AppSettingStore.CurrentValue.ProcessReCalcTimeOut;

        if (!AppSettingStore.CurrentValue.ManualMode)
        {
            UpdateTimer.Start();
            RefreshTimer.Start();
        }
    }

    protected void StopTimers()
    {
        UpdateTimer.Stop();
        RefreshTimer.Stop();
    }
    
    /// <summary>
    ///     Остановка выбранного процесса
    /// </summary>
    public void StopCurrentProcess()
    {
        try
        {
            CurrentProcess.Kill();
            Processes.Remove(CurrentProcess);
        }
        catch (Exception e)
        {
            this.Log().StructLogError($"Process {CurrentProcess.ProcessName} not killed", e.Message);
            return;
        }
        
        this.Log().StructLogInfo($"Process {CurrentProcess.ProcessName} was killed");
    }
    
    public void UpdateProcesses(bool alsoRefresh = true, bool setSubscriptions = true)
    {
        if (!CanReCalc)
        {
            return;
        }
        
        CurrentProcess = default!;
        
        if (!AppSettingStore.CurrentValue.Agreement)
        {
            return;
        }
        
        CanReCalc = false;

        new Action(UpdateProcessesCore).TimeLog(this.Log(),nameof(UpdateProcessesCore));
        
        CanReCalc = true;

        if (alsoRefresh)
        {
            RefreshProcess(); 
        }

        if (setSubscriptions)
        {
            // Обновление подписок
            SetUpdateSubscribes();
        }
        
        InvokeProcessSubscriptions();
    }

    /// <summary>
    ///     Обновление списка процессов 
    /// </summary>
    protected abstract void UpdateProcessesCore();
    
    public void RefreshProcess()
    {
        if (!AppSettingStore.CurrentValue.Agreement)
        {
            return;
        }
        
        if (!CanReCalc)
        {
            return;
        }

        CanReCalc = false;

        new Action(async () =>
        {
            ParallelOptions parallelOptions = new()
            {
                MaxDegreeOfParallelism = -1
            };
            
            await Parallel.ForEachAsync(Processes, parallelOptions, (process, _) =>
            {
                process.Refresh(ComputerInfoService);
                return ValueTask.CompletedTask;
            });
            
        }).TimeLog(this.Log());

        CanReCalc = true;
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
