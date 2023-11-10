namespace Client.Services.AppInfoService;

public interface IAppInfoService
{
    /// <summary>
    ///     Запущено ли приложение в режиме одобрения администратором
    /// </summary>
    public bool IsAdminMode { get; }
}