using AppInfrastructure.Stores.DefaultStore;
using Client.Models;

namespace Client.Stores;

internal sealed class AppSettingsStore : BaseLazyStore<AppSettings>
{
    public AppSettingsStore()
    {
        CurrentValue = new AppSettings();
    }
}