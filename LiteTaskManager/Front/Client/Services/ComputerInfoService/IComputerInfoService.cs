namespace Client.Services.ComputerInfoService;

/// <summary>
///     Характеристики компьютера
/// </summary>
public interface IComputerInfoService
{
    /// <summary>
    ///     Физическая память ПК
    /// </summary>
    public double TotalPhysicalMemoryBytes { get; protected set; }
}