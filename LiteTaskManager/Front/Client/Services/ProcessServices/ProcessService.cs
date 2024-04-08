using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
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
    public ObservableCollection<TaskProcess> Processes { get; private set; } = new ();
    
    [Reactive]
    public TaskProcess CurrentProcess { get; set; } = null!;

    public event Action? ProcessesChanged;
    
    #endregion

    #region Fiels
    
    private IDisposable? _processDisposable;
    
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
    private async void SetUpdateSubscribes()
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
            await Task.Run(UpdateProcesses);
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
    
    public void UpdateProcesses()
    {
        CurrentProcess = null!;
        
        if (!_appSettingStore.CurrentValue.Agreement)
        {
            return;
        }
        
        _canReCalc = false;

        if (OperatingSystem.IsWindows())
        {
            UpdateProcessWindows();
        }
        else
        {
            UpdateProcessUnix();
        }

        _canReCalc = true;

        RefreshProcess();
        
        // Обновление подписок
        SetUpdateSubscribes();
        InvokeProcessSubscriptions();
    }

    /// <summary>
    ///     Обновление списка процессов для винды
    /// </summary>
    private void UpdateProcessWindows()
    {
        new Action(() =>
        {
            if (!OperatingSystem.IsWindows())
            {
                throw new NotSupportedException("Method invoked in not supported os");
            }
            
            // 1 итерация, собираем все процессы в один список
            var buffer = new Dictionary<int, TaskProcess>();
            foreach (var process in Process.GetProcesses())
            {
                var taskProcess = new TaskProcess(process);
                
                buffer.Add(taskProcess.ProcessId,  taskProcess);
            }

            var childsIdx = new List<int>();
            // 2 итерация, распределяем процессы по родителям
            foreach (var taskProcess in buffer.Values.Where(taskProcess => taskProcess.ParentId != 0))
            {
                var getParent =  buffer.TryGetValue(taskProcess.ParentId, out var parentTaskProcess);
                
                if (!getParent)
                {
                    try
                    {
                        var parent =   Process.GetProcessById(taskProcess.ParentId);
                        parentTaskProcess = new TaskProcess(parent);
                        buffer.Add(parentTaskProcess.ProcessId, parentTaskProcess);
                    }
                    catch
                    {
                        this.Log().StructLogDebug("Can't get parent process");
                        continue;
                    }
                }

                parentTaskProcess.Childs.Add(taskProcess);
                
                childsIdx.Add(taskProcess.ProcessId);
            }
            
            // 3. Удаляем лишние
            foreach (var processId in childsIdx)
            {
                buffer.Remove(processId);
            }
            
            Processes = new ObservableCollection<TaskProcess>(buffer.Values);
            
        }).TimeLog(this.Log());
    }
    
    /// <summary>
    ///     Обновление списка процессов для Unix систем
    /// </summary>
    private void UpdateProcessUnix()
    {
        new Action(() =>
        {
            var buffer = new HashSet<TaskProcess>();
            foreach (var process in Process.GetProcesses())
            {
                var taskProcess = new TaskProcess(process);
                buffer.Add(taskProcess);
            }
            
            Processes = new ObservableCollection<TaskProcess>(buffer);
            
        }).TimeLog(this.Log());
    }
    
    public void RefreshProcess()
    {
        if (!_appSettingStore.CurrentValue.Agreement)
        {
            return;
        }
        
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
