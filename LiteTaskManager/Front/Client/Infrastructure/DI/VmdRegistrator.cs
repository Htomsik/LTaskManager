using Client.ViewModels;
using Splat;

namespace Client.Infrastructure.DI;

/// <summary>
///     Класс содержащий в себе методы регистррирующие реализации классов в DI контейнере
/// </summary>
internal static partial class SplatContainerRegistration
{
    public static void VmdRegistration()
    {
        SplatRegistrations.RegisterLazySingleton<MainWindowViewModel>();
        SplatRegistrations.Register<ProcessesViewModel>();
        SplatRegistrations.Register<AppSettingsViewModel>();
        SplatRegistrations.Register<AppInfoViewModel>();
        SplatRegistrations.RegisterLazySingleton<StatusBarViewModel>();
        SplatRegistrations.Register<DisclaimerLiabilityAgreementViewModel>();
    }
} 