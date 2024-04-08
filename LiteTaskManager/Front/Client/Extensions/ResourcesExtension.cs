using Avalonia;
using Avalonia.Media;

namespace Client.Extensions;

/// <summary>
///     Расширение для получения ресурсов из приложения
/// </summary>
public static class ResourcesExtension
{
    #region Brushes
    
    /// <summary>
    ///     Получает кисть из ресурсов и записывает сразу в бекграуд
    /// </summary>
    private static IBrush? GetBrush(string name, ref IBrush? value)
    {
        if (value is not null)
        {
            return value;
        }

        var found =  Application.Current!.TryGetResource(name, Application.Current.ActualThemeVariant, out var brush);

        if (found)
        {
            value = (IBrush?)brush;
        }
            
        return value;
    }
    
    public static IBrush? Percent75Brush => GetBrush("Percent75", ref _percent75Brush);
    
    private static IBrush? _percent75Brush;
    
    
    public static IBrush? Percent50Brush => GetBrush("Percent50", ref _percent50Brush);
    
    private static IBrush? _percent50Brush;
    
    
    public static IBrush? Percent25Brush => GetBrush("Percent25", ref _percent25Brush);
    
    private static IBrush? _percent25Brush;
    
    public static IBrush? Percent0Brush => GetBrush("Percent0", ref _percent0Brush);
    
    private static IBrush? _percent0Brush;
    
    #endregion

    #region Templates names

    /// <summary>
    ///     Шаблон вывода % CPU
    /// </summary>
    public static readonly string ProcessCpuUsagePercentTemplate = "ProcessCpuUsagePercentTemplate";
    
    /// <summary>
    ///     Шаблон вывода % RAM
    /// </summary>
    public static readonly string ProcessRamUsagePercentTemplate = "ProcessRamUsagePercentTemplate";

    /// <summary>
    ///     Шаблон выбора приоритета процесса
    /// </summary>
    public static readonly string ProcessPriorityTemplate = "ProcessPriorityTemplate";

    #endregion
}