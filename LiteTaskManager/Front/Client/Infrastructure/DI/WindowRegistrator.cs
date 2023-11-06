using System;
using Client.Infrastructure.Logging;
using Client.Views;
using Splat;

namespace Client.Infrastructure.DI;

internal static partial class SplatContainerRegistration
{
    public static void WindowRegistration()
    {
        SplatRegistrations.RegisterLazySingleton<IObserver<Exception>, GlobalExceptionHandler>();
        
        SplatRegistrations.RegisterLazySingleton<MainWindow>();
    }
}