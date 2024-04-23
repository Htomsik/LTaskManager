using System;
using Client.Models;

namespace Client.Services.AppCultureService;

/// <summary>
///     Управление локализацией (языком) в приложении
/// </summary>
internal interface IAppCultureService
{
    /// <summary>
    ///     Установка локализации в приложении
    /// </summary>
    public bool SetCulture(AppCulture appCulture);
    
    /// <summary>
    ///     Уведомитель об изменении языка
    /// </summary>
    public event Action? CurrentCultureChangedNotifier;
}