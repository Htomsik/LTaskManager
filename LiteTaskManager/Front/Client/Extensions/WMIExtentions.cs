namespace Client.Extensions;

/// <summary>
///     WMI параметр
/// </summary>
public static class WMIExtentions
{
    /// <summary>
    ///     Процессор
    /// </summary>
    public static (string, string) Processor = ("Win32_Processor", "Name");
    
    /// <summary>
    ///     Производитель процессора
    /// </summary>
    public static (string, string) ProcessorManufacturer = ("Win32_Processor", "Manufacturer");
    
    /// <summary>
    ///     Описание процессора
    /// </summary>
    public static (string, string) ProcessorDescription = ("Win32_Processor", "Description");
    
    /// <summary>
    ///     Видеокарта
    /// </summary>
    public static (string, string) Videocard = ("Win32_VideoController", "Name");
    
    /// <summary>
    ///     Драйвер видеокарты
    /// </summary>
    public static (string, string) VideocardDriver = ("Win32_VideoController", "DriverVersion");
    
    /// <summary>
    ///     Память видеокарты
    /// </summary>
    public static (string, string) VideocardRAM = ("Win32_VideoController", "AdapterRAM");
    
    /// <summary>
    ///     Жесткий диск
    /// </summary>
    public static (string, string) Disk = ("Win32_DiskDrive", "Caption");
    
    /// <summary>
    ///     Объем жесткого диска
    /// </summary>
    public static (string, string) DiskSize = ("Win32_DiskDrive", "Size");
    
    /// <summary>
    ///     Объем ОЗУ
    /// </summary>
    public static (string, string) TotalPhysicalRAM = ("Win32_OperatingSystem", "TotalVisibleMemorySize");
    
    /// <summary>
    ///     Текущее ОЗУ
    /// </summary>
    public static (string, string) FreePhysicalRAM = ("Win32_OperatingSystem", "FreePhysicalMemory");
}