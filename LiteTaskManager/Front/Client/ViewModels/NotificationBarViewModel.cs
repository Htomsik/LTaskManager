
using System;
using AppInfrastructure.Stores.DefaultStore;
using Client.Models;
using Client.Services.AppInfoService.Base;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Client.ViewModels;

/// <summary>
///     Вьюмодель уведомлений
/// </summary>
internal sealed class NotificationBarViewModel : ViewModelBase
{
    /// <summary>
    ///     Уведомление о том что приложение не в админ моде
    /// </summary>
    [Reactive]
    public bool IsNotAdminMode { get; set; }
    
    /// <summary>
    ///     Уведомление о том что приложение в ручном режиме
    /// </summary>
    [Reactive]
    public bool IsManualMode { get; set; }
    
    private AppSettings _appSettings;
    
    public NotificationBarViewModel(IAppInfoService appInfoService, IStore<AppSettings> appSettings)
    {
        IsNotAdminMode = !appInfoService.IsAdminMode;
        _appSettings = appSettings.CurrentValue;
        
        appSettings.CurrentValueChangedNotifier += () =>
        {
            IsManualMode = appSettings.CurrentValue.ManualMode;
            _appSettings = appSettings.CurrentValue;
            
            this.WhenAnyValue(x => x._appSettings.ManualMode).Subscribe(manual =>  IsManualMode = manual);
        };

    }
}