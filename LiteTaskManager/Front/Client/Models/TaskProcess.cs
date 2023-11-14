using System;
using System.Diagnostics;

namespace Client.Models;

/// <summary>
///     Модель процессов Windows
/// </summary>
public class TaskProcess
{
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
    public ProcessPriorityClass PriorityClassCore { get; set; }
    
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
    public ProcessModuleCollection Modules { get; set; } 

    private readonly Process _windowsProcess;
    
    public TaskProcess(){}

    public TaskProcess(Process windowsProcess)
    {
        try
        {
            _windowsProcess = windowsProcess;
            ProcessName = windowsProcess.ProcessName;
            StartTime = windowsProcess.StartTime;
            TotalProcessorTime = windowsProcess.TotalProcessorTime;
            PriorityClassCore = windowsProcess.PriorityClass;
            Modules = windowsProcess.Modules;
            ProductName = windowsProcess.MainModule.FileVersionInfo.ProductName;
            ModuleName = windowsProcess.MainModule.ModuleName;
            CompanyName = windowsProcess.MainModule.FileVersionInfo.CompanyName;
            FileName = windowsProcess.MainModule.FileName;
            ProductVersion = windowsProcess.MainModule.FileVersionInfo.ProductVersion;
        }
        catch (Exception e)
        {
            //ToDo Допилить логирование
        }
    }

    /// <summary>
    ///     Остановка процесса
    /// </summary>
    public void Kill()
    {
        _windowsProcess.Kill();
    }
}