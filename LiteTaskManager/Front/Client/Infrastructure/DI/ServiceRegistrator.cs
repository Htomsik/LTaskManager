using System;
using AppInfrastructure.Services.FileService;
using AppInfrastructure.Services.ParserService;
using AppInfrastructure.Stores.DefaultStore;
using Client.Infrastructure.Logging;
using Client.Models;
using Client.Models.TaskProcess;
using Client.Services;
using Client.Services.AppCultureService;
using Client.Services.AppInfoService;
using Client.Services.Base;
using Client.Services.ComputerInfoService;
using Client.Services.FileServices;
using Client.Services.ParserService;
using Client.Services.WMIService;
using ReactiveUI;
using Splat;

namespace Client.Infrastructure.DI;

/// <summary>
///     Класс содержащий в себе методы регистррирующие реализации классов в DI контейнере
/// </summary>
internal static partial class SplatContainerRegistration
{
    public static void ServiceRegistration()
    {
        SplatRegistrations.RegisterLazySingleton<IObserver<Exception>, GlobalExceptionHandler>();
        
        Locator.CurrentMutable.RegisterLazySingleton(() => new ViewLocator(), typeof(IViewLocator));
        SplatRegistrations.Register<IParserService, FastJsonParserService>();
        SplatRegistrations.Register<IStoreFileService<IStore<AppSettings>, AppSettings>,AppSettingsStoreFileService>();
        SplatRegistrations.Register<IAppInfoService, AppInfoService>();
        SplatRegistrations.Register<IAppCultureService, AppCultureService>();
        SplatRegistrations.Register<IWmiService, WmiService>();
        SplatRegistrations.RegisterLazySingleton<IComputerInfoService, ComputerInfoService>();



        if (OperatingSystem.IsWindows())
        {
            SplatRegistrations.RegisterLazySingleton<IProcessService, WindowsProcessService>();
        }
        else if (OperatingSystem.IsLinux())
        {
            SplatRegistrations.RegisterLazySingleton<IProcessService, UnixProcessService>();
        }
    }
} 