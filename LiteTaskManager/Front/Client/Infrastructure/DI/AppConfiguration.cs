using System;
using System.Runtime.InteropServices;
using Client.Services;
using Client.Services.AppInfoService;
using Client.Services.AppInfoService.Base;
using Client.Services.Base;
using Client.Services.ComputerInfoService;
using Client.Services.ComputerInfoService.Base;
using Serilog;
using Serilog.Events;
using Splat;
using Splat.Serilog;

namespace Client.Infrastructure.DI;

/// <summary>
///     Настройки конфигураации приложения
/// </summary>
internal static class AppConfiguration
{
    public static void Configure()
    {
        SplatContainerRegistration.StoresRegistration();
        SplatContainerRegistration.ServiceRegistration();
        SplatContainerRegistration.VmdRegistration();
        SplatContainerRegistration.WindowRegistration();
        
        var newLogger = new LoggerConfiguration()
            .WriteTo.File(@"logs\Log-.txt", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: LogEventLevel.Information)
            .CreateLogger();

        #if DEBUG
        newLogger = new LoggerConfiguration()
            .WriteTo.File(@"logs\Log-.txt", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: LogEventLevel.Debug)
            .WriteTo.Console()
            .CreateLogger();
        #endif
        
        Locator.CurrentMutable.UseSerilogFullLogger(newLogger);
        
        SplatRegistrations.SetupIOC();
    }
}