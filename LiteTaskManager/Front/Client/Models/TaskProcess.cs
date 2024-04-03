using System;
using System.Diagnostics;
using Client.Extensions;
using Client.Infrastructure.Logging;
using Client.Services.ComputerInfoService;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace Client.Models;

/// <summary>
///     Модель процессов Windows
/// </summary>
public class TaskProcess : ReactiveObject
{
    #region Properties

    /// <summary>
    ///     Наименование которое задал разработчик
    /// </summary>
    public string ProductName { get; set; }

    /// <summary>
    ///     Наименование exe/dll файла
    /// </summary>
    public string ModuleName { get; set; }

    /// <summary>
    ///     Наименование процесса
    /// </summary>
    public string ProcessName { get; set; }

    /// <summary>
    ///     Время от старта процесса
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    ///     Время, затраченное процессором на обработку процесса
    /// </summary>
    public TimeSpan TotalProcessorTime { get; set; }

    /// <summary>
    ///     Приоритет процесса
    /// </summary>
    public ProcessPriorityClass? PriorityClassCore
    {
        get => _priorityClassCore;
        set
        {
            if (value is null)
                return;
            
            if (ChangePriority(value))
                _priorityClassCore = value;
            
            this.RaisePropertyChanged();
        }
    }

    private ProcessPriorityClass? _priorityClassCore;

    /// <summary>
    ///     Компания изготовитель
    /// </summary>
    public string? CompanyName { get; set; }

    /// <summary>
    ///     Путь к исполняемому файлу
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    ///     Версия продукта
    /// </summary>
    public string? ProductVersion { get; set; }
    
    /// <summary>
    ///     Испольемые модули
    /// </summary>
    public ProcessModuleCollection Modules
    {
        get
        {
            try
            {
                return _windowsProcess.Modules;
            }
            catch 
            {
                this.Log().StructLogDebug($"Can't get modules of {ProcessName}");
                
                return new ProcessModuleCollection(new ProcessModule[]{});
            }
        }
    }


    /// <summary>
    ///     Процент использования ОЗУ
    /// </summary>
    [Reactive]
    public double RamUsagePercent { get; set; }
    
    /// <summary>
    ///     Процент использования ЦПУ
    /// </summary>
    [Reactive]
    public double CpuUsagePercent { get; set; }

    #endregion

    /// <summary>
    ///     Счетчик производительности для виндовс
    /// </summary>
    private readonly PerformanceCounter _performanceCounter;

    private readonly Process _windowsProcess;

    public TaskProcess(){}

    public TaskProcess(Process windowsProcess)
    {
        try
        {
            _windowsProcess = windowsProcess;
            ProcessName = windowsProcess.ProcessName;
            StartTime = windowsProcess.StartTime;
            ProductName = windowsProcess.MainModule.FileVersionInfo.ProductName;
            ModuleName = windowsProcess.MainModule.ModuleName;
            CompanyName = windowsProcess.MainModule.FileVersionInfo.CompanyName;
            FileName = windowsProcess.MainModule.FileName;
            ProductVersion = windowsProcess.MainModule.FileVersionInfo.ProductVersion;
        }
        catch 
        {
             this.Log().StructLogDebug($"Don't have access to {windowsProcess.ProcessName}");
        }

        try
        {
            if (OperatingSystem.IsWindows())
            {
                _performanceCounter = new PerformanceCounter(PerfCounterExtentions.ProcessCategory, PerfCounterExtentions.ProcessCpuUsageCounter, ProcessName, true);
            }
        }
        catch
        {
        }
    }

    /// <summary>
    ///     Изменяет приоритет процесса
    /// </summary>
    /// TODO: Подумать о сервисе который будет заниматься операциями с процессами
    public bool ChangePriority(ProcessPriorityClass? processPriorityClass)
    {
        try
        {
            _windowsProcess.PriorityClass = (ProcessPriorityClass)processPriorityClass;
        }
        catch(Exception e)
        {
            this.Log().StructLogError($"Can't change priority of {ProcessName}", e.Message);
            return false;
        }
        return true;
    }

    /// <summary>
    ///     Обновление
    /// </summary>
    public void Refresh()
    {
        _windowsProcess.Refresh();
        
        TotalProcessorTime = _windowsProcess.TotalProcessorTime;
        _priorityClassCore = _windowsProcess.PriorityClass;
    }
    
    /// <summary>
    ///     Перерасчет полей
    /// </summary>
    public void Recalc(IComputerInfoService computerInfoService)
    {
        if (OperatingSystem.IsWindows() && _performanceCounter != null)
        {
            try
            {
                // TODO не спраишивайте меня почему так, я сам не знаю
                if (ProcessName == "Idle")
                {
                    CpuUsagePercent = _performanceCounter.NextValue() / 10000;
                }
                else
                {
                    CpuUsagePercent = _performanceCounter.NextValue() / 100;
                }
            }
            catch 
            {
                this.Log().StructLogDebug(
                    $"Don't have access to {PerfCounterExtentions.ProcessCpuUsageCounter} {ProcessName}");
            }
        }
        
        RamUsagePercent = double.Round(_windowsProcess.WorkingSet64 / computerInfoService.TotalPhysicalMemoryBytes, 1);
    }

    /// <summary>
    ///     Остановка процесса
    /// </summary>
    public void Kill()
    {
        _windowsProcess.Kill();
    }

    public override string ToString()
    {
        return ProcessName;
    }
}