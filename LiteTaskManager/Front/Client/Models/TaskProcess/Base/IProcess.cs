using System;
using System.Collections.Generic;
using System.Diagnostics;
using Client.Services.ComputerInfoService.Base;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Client.Models.TaskProcess.Base;

public interface IProcess : IReactiveObject
{
    /// <summary>
    ///     Id процесса
    /// </summary>
    public int ProcessId { get; }

    /// <summary>
    ///     Id процесса
    /// </summary>
    public int ParentId { get; }
    
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
    public ProcessPriorityClass? PriorityClassCore { get; set; }
    
    /// <summary>
    ///     Компания изготовитель
    /// </summary>
    public string CompanyName { get; set; } 

    /// <summary>
    ///     Путь к исполняемому файлу
    /// </summary>
    public string FileName { get; set; } 

    /// <summary>
    ///     Версия продукта
    /// </summary>
    public string ProductVersion { get; set; } 
    
    /// <summary>
    ///     Испольемые модули
    /// </summary>
    public ProcessModuleCollection Modules { get; }
    
    /// <summary>
    ///     Процент использования ОЗУ
    /// </summary>
    public double RamUsagePercent { get; set; }
    
    /// <summary>
    ///     Процент использования ЦПУ
    /// </summary>
    public double CpuUsagePercent { get; set; }
    
    /// <summary>
    ///     Завершен ли процесс
    /// </summary>
    [Reactive]
    public bool HasExited { get; set; }
    
    /// <summary>
    ///     Запущенные процессом процессы
    /// </summary>
    public ICollection<IProcess> Childs { get; } 
    
    /// <summary>
    ///     Используемые ядра процесса
    /// </summary>
    public ICollection<ProcessAffinityCore> ProcessorAffinity { get; }
    
    /// <summary>
    ///     Обновление данных процесса
    /// </summary>
    /// <param name="computerInfoService">  Информация о системе, нужна для расчета использования   </param>
    /// <param name="includeChilds">    Перерасчитывать ли информацию дочерних процессов    </param>
    public bool Refresh(IComputerInfoService computerInfoService, bool includeChilds = true);
    
    /// <summary>
    ///     Остановка процесса
    /// </summary>
    public bool Kill();
}