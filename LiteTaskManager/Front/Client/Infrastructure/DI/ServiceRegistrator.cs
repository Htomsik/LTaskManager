using System;
using System.Diagnostics;
using Client.Infrastructure.Logging;
using Client.Services;
using ReactiveUI;
using Splat;

namespace Client.Infrastructure.DI;

internal static partial class SplatContainerRegistration
{
    public static void ServiceRegistration()
    {
        SplatRegistrations.RegisterLazySingleton<IObserver<Exception>, GlobalExceptionHandler>();
        SplatRegistrations.Register<IProcessService<Process>, ProcessService>();
        Locator.CurrentMutable.RegisterLazySingleton(() => new ViewLocator(), typeof(IViewLocator));
    }
} 