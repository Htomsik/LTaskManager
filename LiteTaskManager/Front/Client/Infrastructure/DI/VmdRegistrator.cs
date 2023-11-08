using Client.ViewModels;
using Splat;

namespace Client.Infrastructure.DI;

internal static partial class SplatContainerRegistration
{
    public static void VmdRegistration()
    {
        SplatRegistrations.RegisterLazySingleton<MainWindowViewModel>();
        SplatRegistrations.Register<ProcessesViewModel>();
        SplatRegistrations.Register<AppSettingsViewModel>();
    }
} 