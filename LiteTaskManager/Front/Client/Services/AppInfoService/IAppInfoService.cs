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
    
    public string AppName { get; set; }
        
    public string AppManufacturer { get; set; }
        
    public string AppGitHub { get; set; }
        
    public string AppVersion { get; set; }
}