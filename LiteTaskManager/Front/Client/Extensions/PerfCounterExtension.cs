namespace Client.Extensions;

/// <summary>
/// 
/// </summary>
public class PerfCounterExtension
{
    /// <summary>
    ///     Категория взаимодействия с процессами
    /// </summary>
    public static string ProcessCategory = "Process";
    
    /// <summary>
    ///     Процент использования CPU
    /// </summary>
    public static string ProcessCpuUsageCounter = "% Processor Time";
    
    /// <summary>
    ///     Id родительского процесса
    /// </summary>
    public static string ProcessParentId = "Creating Process ID";
}