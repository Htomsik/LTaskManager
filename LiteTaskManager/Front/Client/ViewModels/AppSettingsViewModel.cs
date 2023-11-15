using System;
using AppInfrastructure.Stores.DefaultStore;
using Client.Models;
using Client.Services.AppCultureService;
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
    ///     Сервис смены локализации
    /// </summary>
    private readonly IAppCultureService _appCultureService;
    
    /// <param name="appSettingsStore"> Хранилище настроек приложения </param>
    /// <param name="appCultureService"> Сервси смены  </param>
    public AppSettingsViewModel(IStore<AppSettings> appSettingsStore, IAppCultureService appCultureService)
    {
        _appCultureService = appCultureService;
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
        
        // Подписка на смену культуры в настройках. Как только меняется, применятся метод смены языка в приложении
        _cultureSubscribe = this.WhenAnyValue(x => x.Settings.Culture)
            .Subscribe(culture => _appCultureService.SetCulture(culture));
    }
}