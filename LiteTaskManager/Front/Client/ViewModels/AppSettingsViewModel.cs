using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
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

        appSettingsStore.CurrentValueChangedNotifier += () =>
        {
            Settings = appSettingsStore.CurrentValue;
            SetSubscribes();
        };

        SetSubscribes();
    }

    private void SetSubscribes()
    {
        CompositeDisposable.Dispose();
        
        // Подписка на смену культуры в настройках. Как только меняется, применятся метод смены языка в приложении
        this.WhenAnyValue(x => x.Settings.Culture).Skip(1)
            .Subscribe(culture => _appCultureService.SetCulture(culture))
            .DisposeWith(CompositeDisposable);

        // Подписка на смену закрытия в трей
        this.WhenAnyValue(x => x.Settings.ShutdownToTray)
            .Skip(1)
            .Subscribe(mode =>
            {
                _appTrayService.ChangeShutdownPolitic(mode
                    ? ShutdownMode.OnExplicitShutdown
                    : ShutdownMode.OnMainWindowClose);
            })
            .DisposeWith(CompositeDisposable);
    }
}