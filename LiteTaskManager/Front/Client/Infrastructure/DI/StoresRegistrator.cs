using AppInfrastructure.Stores.DefaultStore;
using Client.Models;
using Client.Stores;
using Splat;

namespace Client.Infrastructure.DI;

internal static partial class SplatContainerRegistration
{
    public static void StoresRegistration()
    {
        SplatRegistrations.RegisterLazySingleton<IStore<AppSettings>,AppSettingsStore>();
    }
} 