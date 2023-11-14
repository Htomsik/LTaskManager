using Serilog;
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
            .WriteTo.File(@"logs\Log-.txt", rollingInterval: RollingInterval.Day)
            .WriteTo.Console()
            .CreateLogger();
        
        Locator.CurrentMutable.UseSerilogFullLogger(newLogger);
        
        SplatRegistrations.SetupIOC();
    }
}