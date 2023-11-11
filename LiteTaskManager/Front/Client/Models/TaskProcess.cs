using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Client.Models;

public class TaskProcess
{
    public string ProductName { get; set; }
    public string ModuleName { get; set; }
    public string ProcessName { get; set; }
    public DateTime StartTime{ get; set; }
    public TimeSpan TotalProcessorTime { get; set; }
    public ProcessPriorityClass PriorityClassCore { get; set; }
    
    public string? CompanyName { get; set; }
    
    public string FileName { get; set; }
    
    public string? ProductVersion { get; set; } 
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

    public void Kill()
    {
        _windowsProcess.Kill();
    }
}