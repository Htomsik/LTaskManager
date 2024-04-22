using System;
using AppInfrastructure.Services.FileService;
using AppInfrastructure.Services.ParserService;
using AppInfrastructure.Stores.DefaultStore;
using Client.Infrastructure.Logging;
using Client.Models;
using Client.Services;
using Client.Services.AppCultureService;
using Client.Services.AppInfoService;
using Client.Services.AppInfoService.Base;
using Client.Services.AppTrayService;
using Client.Services.Base;
using Client.Services.ComputerInfoService;
using Client.Services.ComputerInfoService.Base;
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
        SplatRegistrations.Register<IAppCultureService, AppCultureService>();
        SplatRegistrations.Register<IAppTrayService, AppTrayService>();
        
        ServiceRegistrationWindows();
    }
    
    public static void ServiceRegistrationWindows()
    {
        SplatRegistrations.RegisterLazySingleton<IProcessService, WindowsProcessService>();
        SplatRegistrations.Register<IAppInfoService, WindowsAppInfoService>();
        SplatRegistrations.Register<IWindowsWmiService, WindowsWmiService>();
        SplatRegistrations.RegisterLazySingleton<IComputerInfoService, WindowsComputerInfoService>();
    }
    
    public static void ServiceRegistrationLinux()
    {
        // Раскоментить если надо для линукса и закоментить винду
        // SplatRegistrations.RegisterLazySingleton<IProcessService, UnixProcessService>();
        // SplatRegistrations.Register<IAppInfoService, UnixAppInfoService>();
        // SplatRegistrations.RegisterLazySingleton<IComputerInfoService, UnixComputerInfoService>();
    }
} 