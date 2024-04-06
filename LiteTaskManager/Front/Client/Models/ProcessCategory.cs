namespace Client.Models;

/// <summary>
///     Режимы показа процессов
/// </summary>
public enum ProcessCategory
{
    /// <summary>
    ///     Все
    /// </summary>
    All,
    
    /// <summary>
    ///     Только системные
    /// </summary>
    Systems,
    
    /// <summary>
    ///     Только не системные
    /// </summary>
    NotSystems
}