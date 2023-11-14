using AppInfrastructure.Stores.DefaultStore;
using Client.Models;

namespace Client.Stores;

/// <summary>
///     Хранилише настроек приложения
/// <remarks> Хранится как сингтон инстанс в DI контейнере на все приложение </remarks>
/// </summary>
internal sealed class AppSettingsStore : BaseLazyStore<AppSettings>
{
    public AppSettingsStore()
    {
        CurrentValue = new AppSettings();
    }
}