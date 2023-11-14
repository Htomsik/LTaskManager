using AppInfrastructure.Stores.DefaultStore;
using Client.Models;
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
    
    /// <param name="appSettingsStore"> Хранилище настроек приложения </param>
    public AppSettingsViewModel(IStore<AppSettings> appSettingsStore)
    {
        Settings = appSettingsStore.CurrentValue;
        
        appSettingsStore.CurrentValueChangedNotifier += ()=> Settings = appSettingsStore.CurrentValue ;
    }
}