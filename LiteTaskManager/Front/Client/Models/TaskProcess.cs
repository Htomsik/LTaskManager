using System;
using System.Collections.Generic;

using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Client.Extensions;
using Client.Infrastructure.Logging;
using Client.Services.ComputerInfoService;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace Client.Models;

// TODO : В модели слишком много логики ее управления
// Нужно сделать отдельный сервис который бу управлял текущей моделью
/// <summary>
///     Модель процессов Windows
/// </summary>
public class TaskProcess : ReactiveObject
{
    #region Properties
    
    /// <summary>
    ///     Id процесса
    /// </summary>
    public int ProcessId => _windowsProcess.Id;

    /// <summary>
    ///     Id процесса
    /// </summary>
    public int ParentId => _parentId ??= GetParentId();

    private int? _parentId;

    /// <summary>
    ///     Наименование которое задал разработчик
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    ///     Наименование exe/dll файла
    /// </summary>
    public string ModuleName { get; set; } = string.Empty;

    /// <summary>
    ///     Наименование процесса
    /// </summary>
    public string ProcessName { get; set; } = string.Empty;

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
            if (HasExited)
            {
                return;
            }

            if (value is null)
            {
                return;
            }
            
            if (ChangePriority((ProcessPriorityClass)value))
                _priorityClassCore = value;
            
            this.RaisePropertyChanged();
        }
    }

    private ProcessPriorityClass? _priorityClassCore;

    /// <summary>
    ///     Компания изготовитель
    /// </summary>
    public string CompanyName { get; set; } = string.Empty;

    /// <summary>
    ///     Путь к исполняемому файлу
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    ///     Версия продукта
    /// </summary>
    public string ProductVersion { get; set; } = string.Empty;
    
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
            catch (Exception e)
            {
                this.Log().StructLogDebug($"Can't get modules of {ProcessName}", e.Message);
                
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
    
    /// <summary>
    ///     Завершен ли процесс
    /// </summary>
    [Reactive]
    public bool HasExited { get; set; }
    
    
    public List<TaskProcess> Childs { get; } = new();


    /// <summary>
    ///     Используемые ядра процесса
    /// </summary>
    public ObservableCollection<ProcessAffinityCore> ProcessorAffinity
    {
        get
        {
            try
            {
                var chars = Convert.ToString(_windowsProcess.ProcessorAffinity, 2).ToList();
            
                var affinity = new ObservableCollection<ProcessAffinityCore>();
            
                for (var index = 0; index < chars.Count; index++)
                {
                    affinity.Add(new ProcessAffinityCore(index, chars[index] == '1'));
                }
            
                return affinity;
            }
            catch (Exception e)
            {
                this.Log().StructLogDebug("Can't get affinity", e.Message);
                return new ObservableCollection<ProcessAffinityCore>();
            }
        }
    }


    #endregion

    #region Fields

    /// <summary>
    ///     Выполнен ли первый проход perfomanceCounter
    /// </summary>
    private bool _perfRefreshed;

    #endregion

    /// <summary>
    ///     Счетчик производительности для виндовс
    /// </summary>
    private readonly PerformanceCounter? _performanceCounterCpuUsage;
    
    /// <summary>
    ///     Счетчик производительности для виндовс
    /// </summary>
    private readonly PerformanceCounter? _performanceCounterParentId;

    private readonly Process _windowsProcess = new ();
    
    public TaskProcess(Process windowsProcess)
    {
        try
        {
            _windowsProcess = windowsProcess;
            
            // Данные которые точно можно получить 
            ProcessName = windowsProcess.ProcessName;
            StartTime = windowsProcess.StartTime;
            
            // Есть риск что выйдет ошибка 
            ModuleName = windowsProcess.MainModule?.ModuleName ?? string.Empty;
            FileName = windowsProcess.MainModule?.FileName ?? string.Empty;
            ProductName = windowsProcess.MainModule?.FileVersionInfo.ProductName ?? string.Empty;
            CompanyName = windowsProcess.MainModule?.FileVersionInfo.CompanyName ?? string.Empty;
            ProductVersion = windowsProcess.MainModule?.FileVersionInfo.ProductVersion ?? string.Empty;
        }
        catch (Exception e)
        {

             this.Log().StructLogDebug($"Don't have access to {windowsProcess.ProcessName}", e.Message);
        }

        try
        {
            if (OperatingSystem.IsWindows())
            {
                _performanceCounterCpuUsage = new PerformanceCounter(PerfCounterExtension.ProcessCategory, PerfCounterExtension.ProcessCpuUsageCounter, ProcessName, true);
                _performanceCounterParentId = new PerformanceCounter(PerfCounterExtension.ProcessCategory, PerfCounterExtension.ProcessParentId, ProcessName, true);
            }
        }
        catch (Exception e)
        {
            this.Log().StructLogDebug($"Can't create {_performanceCounterCpuUsage}", e.Message);
        }
    }

    #region Методы взаимодействия с процессом

    /// <summary>
    ///     Изменяет приоритет процесса
    /// </summary>
    /// TODO: Подумать о сервисе который будет заниматься операциями с процессами
    private bool ChangePriority(ProcessPriorityClass processPriorityClass)
    {
        if (_windowsProcess.PriorityClass == processPriorityClass)
        {
            return false;
        }
        
        try
        {
            _windowsProcess.PriorityClass = processPriorityClass;
        }
        catch(Exception e)
        {
            this.Log().StructLogError($"Can't change priority of {ProcessName}", e.Message);
            return false;
        }
        
        return true;
    }

    /// <summary>
    ///     Обновление данных
    /// </summary>
    public bool Refresh(IComputerInfoService computerInfoService, bool includeChilds = true)
    {
        if (includeChilds)
        {
            foreach (var elem in Childs)
            {
                elem.Refresh(computerInfoService, true);
            }
        }
        
        // Если процесс завершен нет смысла его считать
        if (HasExited)
        {
            return false;
        }
        
        try
        {
            _windowsProcess.Refresh();
        
            HasExited = _windowsProcess.HasExited;
            TotalProcessorTime = _windowsProcess.TotalProcessorTime;
            _priorityClassCore = _windowsProcess.PriorityClass;
            
        }
        catch (Exception e)
        {
            this.Log().StructLogDebug($"Can't Refresh {ProcessName}", e.Message);
        }
        
        if (HasExited)
        {
            ReCalcClear();
            return false;
        }
        
        ReCalc(computerInfoService);

        return true;
    }
    
    /// <summary>
    ///     Остановка процесса
    /// </summary>
    public void Kill()
    {
        _windowsProcess.Kill();
    }

    #endregion
    
    #region ReCalc методы перерасчета

    /// <summary>
    ///     Перерасчет 
    /// </summary>
    private void ReCalc(IComputerInfoService computerInfoService)
    {
        ReCalCpuUsage();
        
        try
        {
            RamUsagePercent = double.Round((_windowsProcess.WorkingSet64 / computerInfoService.TotalPhysicalMemoryBytes) * 100, 2);
        }
        catch (Exception e)
        {
            this.Log().StructLogDebug(
                $"Don't have access to {PerfCounterExtension.ProcessCpuUsageCounter} {ProcessName}", e.Message);
        }
    }

    /// <summary>
    ///     Перерасчет использования CPU
    /// </summary>
    private void ReCalCpuUsage()
    {
        if (!OperatingSystem.IsWindows()) return;

        if (_performanceCounterCpuUsage is null)
        {
            this.Log().StructLogDebug($"{nameof(_perfRefreshed)} not initialized");
            return;
        }
        
        if (!_perfRefreshed)
        {
            _perfRefreshed = true;
            _performanceCounterCpuUsage.NextValue();
        }
        
        try
        {
            double cpuUsage; 
            
            // TODO не спраишивайте меня почему так, я сам не знаю
            if (ProcessName == ProcessExtension.ProcessIdle)
            {
                cpuUsage = _performanceCounterCpuUsage.NextValue() / 10000;
            }
            else
            {
                cpuUsage = _performanceCounterCpuUsage.NextValue() / 100;
            }

            CpuUsagePercent = double.Round(cpuUsage * 100, 2);
        }
        catch (Exception e)
        {
            this.Log().StructLogDebug(
                $"Don't have access to {PerfCounterExtension.ProcessCpuUsageCounter} {ProcessName}", e.Message);
        }
    }

    /// <summary>
    ///     Очистка перерасчитываемых значений
    /// </summary>
    private void ReCalcClear()
    {
        CpuUsagePercent = 0;
        RamUsagePercent = 0;
    }

    /// <summary>
    ///     Получение Id родителя
    /// </summary>
    private int GetParentId()
    {
        if (!OperatingSystem.IsWindows())
        {
            return 0;
        }
        
        if (_performanceCounterParentId is null)
        {
            this.Log().StructLogDebug($"{nameof(_perfRefreshed)} not initialized");
            return 0;
        }
        
        return (int)_performanceCounterParentId.NextValue();
    }

    #endregion
    
    public override string ToString()
    {
        return ProcessName;
    }
}