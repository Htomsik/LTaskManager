using AppInfrastructure.Stores.DefaultStore;
using Client.Models;
using Client.Stores;
using Splat;

namespace Client.Infrastructure.DI;

/// <summary>
///     Класс содержащий в себе методы регистррирующие реализации классов в DI контейнере
/// </summary>
internal static partial class SplatContainerRegistration
{
    public static void StoresRegistration()
    {
        SplatRegistrations.RegisterLazySingleton<IStore<AppSettings>,AppSettingsStore>();
    }
} 