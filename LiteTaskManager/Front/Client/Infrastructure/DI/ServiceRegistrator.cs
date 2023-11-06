using System;
using Client.Infrastructure.Logging;
using ReactiveUI;
using Splat;

namespace Client.Infrastructure.DI;

internal static partial class SplatContainerRegistration
{
    public static void ServiceRegistration()
    {
        SplatRegistrations.RegisterLazySingleton<IObserver<Exception>, GlobalExceptionHandler>();
        Locator.CurrentMutable.RegisterLazySingleton(() => new ViewLocator(), typeof(IViewLocator));
    }
} 