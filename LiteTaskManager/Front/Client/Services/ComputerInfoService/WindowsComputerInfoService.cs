using Client.Extensions;
using Client.Services.ComputerInfoService.Base;
using Client.Services.WMIService;

namespace Client.Services.ComputerInfoService;

/// <summary>
///   <seealso cref="IComputerInfoService"/>
/// </summary>
internal sealed class WindowsComputerInfoService : BaseComputerInfoService
{
    private readonly IWindowsWmiService _windowsWmiService;

    public WindowsComputerInfoService(IWindowsWmiService windowsWmiService) 
    {
        _windowsWmiService = windowsWmiService;
        
        InitData();
    }

    protected override void InitData()
    {
        var data = _windowsWmiService.GetHardwareInfo(WmiExtension.TotalPhysicalRAM.Item1, WmiExtension.TotalPhysicalRAM.Item2);

        if (double.TryParse(data, out var physicalMemory))
        {
            // По непонятной причине возвращает кбайты
            TotalPhysicalMemoryBytes = physicalMemory * 1024;
        }
        
        data = _windowsWmiService.GetHardwareInfo(WmiExtension.ProcessorThreadNumbers.Item1, WmiExtension.ProcessorThreadNumbers.Item2);
         
        if (int.TryParse(data, out var cpuThreadCount))
        {
            CpuThreadCount = cpuThreadCount;
        }
    }
}