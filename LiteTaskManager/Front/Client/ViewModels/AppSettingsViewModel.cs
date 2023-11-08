using AppInfrastructure.Stores.DefaultStore;
using Client.Models;
using ReactiveUI.Fody.Helpers;

namespace Client.ViewModels;

internal sealed class AppSettingsViewModel : ViewModelBase
{
    [Reactive]
    public AppSettings Settings { get; set; }
    
    public AppSettingsViewModel(IStore<AppSettings> appSettingsStore)
    {
        Settings = appSettingsStore.CurrentValue;
        
        appSettingsStore.CurrentValueChangedNotifier += ()=> Settings = appSettingsStore.CurrentValue ;
    }
}