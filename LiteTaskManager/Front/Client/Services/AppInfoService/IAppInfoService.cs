namespace Client.Services.AppInfoService;

/// <summary>
///     Предоставляет информацию о текущем запущенном приложении
/// </summary>
public interface IAppInfoService
{
    /// <summary>
    ///     Запущено ли приложение в режиме одобрения администратором
    /// </summary>
    public bool IsAdminMode { get; }
    
    public string AppName { get; }
        
    public string AppManufacturer { get; }
        
    public string AppGitHub { get; }
        
    public string AppVersion { get; }
}