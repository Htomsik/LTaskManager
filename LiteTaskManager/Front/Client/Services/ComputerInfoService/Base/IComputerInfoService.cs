namespace Client.Services.ComputerInfoService.Base;

/// <summary>
///     Характеристики компьютера
/// </summary>
public interface IComputerInfoService
{
    /// <summary>
    ///     Физическая память ПК
    /// </summary>
    public double TotalPhysicalMemoryBytes { get; protected set; }
    
    /// <summary>
    ///     Количество потоков процессора
    /// </summary>
    public int CpuThreadCount { get; protected set; }
}