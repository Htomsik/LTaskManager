using System;
using AppInfrastructure.Stores.DefaultStore;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Client.Models;
using Client.Services.AppCultureService;
using Client.Services.AppTrayService;
using Microsoft.Extensions.Hosting.Internal;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Client.ViewModels;

/// <summary>
///     Вьюмодель настроек приложения
/// </summary>
internal sealed class AppSettingsViewModel : ViewModelBase
{
    /// <summary>
    ///     Текущие настройки приложения
    /// </summary>
    [Reactive]
    public AppSettings Settings { get; set; }

    /// <summary>
    ///     Подписка на изменение культуры в хранилише настроек
    /// </summary>
    private IDisposable? _cultureSubscribe;

    /// <summary>
    ///     Подписка на смену закрытия в трей
    /// </summary>
    private IDisposable? _shutDownSubscribe;
    
    /// <summary>
    ///     Сервис смены локализации
    /// </summary>
    private readonly IAppCultureService _appCultureService;

    /// <summary>
    ///     Сервсис управления треем приложения
    /// </summary>
    private readonly IAppTrayService _appTrayService;

    /// <param name="appSettingsStore"> Хранилище настроек приложения </param>
    /// <param name="appCultureService"> Сервси смены языка </param>
    /// <param name="appTrayService"> Управление треем прилоения </param>
    public AppSettingsViewModel(IStore<AppSettings> appSettingsStore, 
        IAppCultureService appCultureService,
        IAppTrayService appTrayService)    
    {
        _appCultureService = appCultureService;
        _appTrayService = appTrayService;
        
        Settings = appSettingsStore.CurrentValue;
        
        appSettingsStore.CurrentValueChangedNotifier += ()=>
        {
            Settings = appSettingsStore.CurrentValue;
            SetSubscribes();
        };
        
        SetSubscribes();
    }

    private void SetSubscribes()
    {
        _cultureSubscribe?.Dispose();
        _shutDownSubscribe?.Dispose();
        
        // Подписка на смену культуры в настройках. Как только меняется, применятся метод смены языка в приложении
        _cultureSubscribe = this.WhenAnyValue(x => x.Settings.Culture)
            .Subscribe(culture => _appCultureService.SetCulture(culture));

        // Подписка на смену закрытия в трей
        _shutDownSubscribe = this.WhenAnyValue(x => x.Settings.ShutdownToTray)
            .Subscribe(mode =>
            {
                _appTrayService.ChangeShutdownPolitic(mode ? ShutdownMode.OnExplicitShutdown : ShutdownMode.OnMainWindowClose);
            });
    }
}