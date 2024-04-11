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

        if (double.TryParse(data, out var parsed))
        {
            // По непонятной причине возвращает кбайты
            TotalPhysicalMemoryBytes = parsed * 1024;
        }
    }
}