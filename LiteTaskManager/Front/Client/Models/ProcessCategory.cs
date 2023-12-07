namespace Client.Models;

/// <summary>
///     Режимы показа процессов
/// </summary>
public enum ProcessCategory
{
    /// <summary>
    ///     Все
    /// </summary>
    all,
    
    /// <summary>
    ///     Только системные
    /// </summary>
    systems,
    
    /// <summary>
    ///     Только не системные
    /// </summary>
    notSystems
}