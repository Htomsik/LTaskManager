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