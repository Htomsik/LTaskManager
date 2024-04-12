using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Client.Extensions;
using Client.Infrastructure.Logging;
using Client.Services.ComputerInfoService.Base;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace Client.Models.TaskProcess.Base;

// TODO : В модели слишком много логики ее управления
// Нужно сделать отдельный сервис который бу управлял текущей моделью
/// <summary>
///     Базовая модель процессов общая для всех платформ
/// </summary>
public abstract class BaseProcess : ReactiveObject, IProcess
{
    #region Properties
    
    /// <summary>
    ///     Id процесса
    /// </summary>
    public int ProcessId => Process.Id;

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
                return Process.Modules;
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


    public ICollection<IProcess> Childs { get; } = new List<IProcess>();


    /// <summary>
    ///     Используемые ядра процесса
    /// </summary>
    public ICollection<ProcessAffinityCore> ProcessorAffinity => GetProcessorAffinity();
    
    
    #endregion

    #region Fields
    
    /// <summary>
    ///     Основной процесс
    /// </summary>
    protected readonly Process Process = new ();

    #endregion

    #region Constructors

    public BaseProcess(Process process)
    {
        try
        {
            Process = process;
            
            // Данные которые точно можно получить 
            ProcessName = process.ProcessName;
            StartTime = process.StartTime;
            
            // Есть риск что выйдет ошибка 
            ModuleName = process.MainModule?.ModuleName ?? string.Empty;
            FileName = process.MainModule?.FileName ?? string.Empty;
            ProductName = process.MainModule?.FileVersionInfo.ProductName ?? string.Empty;
            CompanyName = process.MainModule?.FileVersionInfo.CompanyName ?? string.Empty;
            ProductVersion = process.MainModule?.FileVersionInfo.ProductVersion ?? string.Empty;
        }
        catch (Exception e)
        {

            this.Log().StructLogDebug($"Don't have access to {process.ProcessName}", e.Message);
        }
    }

    #endregion
    
    #region Methods

    #region Operations
    
    /// <summary>
    ///     Получение Id родителя
    /// </summary>
    protected abstract int GetParentId();

    protected abstract ICollection<ProcessAffinityCore> GetProcessorAffinity();

    public virtual bool Kill()
    {
        if (HasExited)
        {
            return true;
        }
        
        try
        {
            Process.Kill();
        }
        catch (Exception e)
        {
            this.Log().StructLogError($"Can't kill process {ProcessName}", e.Message);
            return false;
        }

        HasExited = true;

        return true;
    }
    
    /// <summary>
    ///     Изменяет приоритет процесса
    /// </summary>
    protected virtual bool ChangePriority(ProcessPriorityClass processPriorityClass)
    {
        if (Process.PriorityClass == processPriorityClass)
        {
            return false;
        }
        
        try
        {
            Process.PriorityClass = processPriorityClass;
        }
        catch(Exception e)
        {
            this.Log().StructLogError($"Can't change priority of {ProcessName}", e.Message);
            return false;
        }
        
        return true;
    }

    #endregion
    
    #region Recacl methods
    
    public bool Refresh(IComputerInfoService computerInfoService, bool includeChilds = true)
    {
        if (includeChilds)
        {
            foreach (var elem in Childs.ToArray())
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
            Process.Refresh();
        
            HasExited = Process.HasExited;
            TotalProcessorTime = Process.TotalProcessorTime;
            _priorityClassCore = Process.PriorityClass;
            
        }
        catch (Exception e)
        {
            this.Log().StructLogDebug($"Can't Refresh {ProcessName}", e.Message);
        }
        
        if (HasExited)
        {
            ClearUsages();
            return false;
        }
        
        ReCalcRamUsage(computerInfoService);
        ReCalCpuUsage();

        return true;
    }
    
    /// <summary>
    ///     Перерасчет использования CPU
    /// </summary>
    protected abstract void ReCalCpuUsage();
    
    /// <summary>
    ///     Перерасчет использования RAM
    /// </summary>
    /// <param name="computerInfoService">  Информация о системе, нужна для расчета использования </param>
    protected virtual void ReCalcRamUsage(IComputerInfoService computerInfoService)
    {
        if (computerInfoService.TotalPhysicalMemoryBytes == 0)
        {
            this.Log().StructLogFatal(
                $"Can't calculate RAM usage becasude {nameof(computerInfoService.TotalPhysicalMemoryBytes)} is 0");
            return;
        }
        
        try
        {
            RamUsagePercent = double.Round((Process.WorkingSet64 / computerInfoService.TotalPhysicalMemoryBytes) * 100, 2);
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
    private void ClearUsages()
    {
        CpuUsagePercent = 0;
        RamUsagePercent = 0;
    }

    #endregion
    
    #endregion
    
    public override string ToString()
    {
        return ProcessName;
    }
}