using System;
using AppInfrastructure.Services.FileService;
using AppInfrastructure.Services.ParserService;
using AppInfrastructure.Stores.DefaultStore;
using Client.Infrastructure.Logging;
using Client.Models;
using Client.Services;
using Client.Services.AppCultureService;
using Client.Services.AppInfoService;
using Client.Services.FileServices;
using Client.Services.ParserService;
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
        SplatRegistrations.RegisterLazySingleton<IProcessService<TaskProcess>, ProcessService>();
        Locator.CurrentMutable.RegisterLazySingleton(() => new ViewLocator(), typeof(IViewLocator));
        SplatRegistrations.Register<IParserService, FastJsonParserService>();
        SplatRegistrations.Register<IStoreFileService<IStore<AppSettings>, AppSettings>,AppSettingsStoreFileService>();
        SplatRegistrations.Register<IAppInfoService, AppInfoService>();
        SplatRegistrations.Register<IAppCultureService, AppCultureService>();
    }
} 