using Avalonia.Controls;

namespace Client.Services.AppTrayService;

/// <summary>
///     Сервис управления треем приложения
/// </summary>
internal  interface IAppTrayService
{
    /// <summary>
    ///     Открыть главное окно приложения
    /// </summary>
    public void ShowWindow();

    /// <summary>
    ///     Закрыть приложение
    /// </summary>
    public void CloseApp();

    /// <summary>
    ///     Сменить политику закрытия главного окноп
    /// </summary>
    public void ChangeShutdownPolitic(ShutdownMode mode);
}