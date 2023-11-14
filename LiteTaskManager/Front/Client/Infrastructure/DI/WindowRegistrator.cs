using System;
using Client.Infrastructure.Logging;
using Client.Views;
using Splat;

namespace Client.Infrastructure.DI;

/// <summary>
///     Класс содержащий в себе методы регистррирующие реализации классов в DI контейнере
/// </summary>
internal static partial class SplatContainerRegistration
{
    public static void WindowRegistration()
    {
        SplatRegistrations.RegisterLazySingleton<IObserver<Exception>, GlobalExceptionHandler>();
        
        SplatRegistrations.RegisterLazySingleton<MainWindow>();
    }
}