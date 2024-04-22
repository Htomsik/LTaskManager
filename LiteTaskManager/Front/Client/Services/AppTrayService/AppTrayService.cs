using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Client.Infrastructure.Logging;
using Client.ViewModels;
using Client.Views;
using Splat;

namespace Client.Services.AppTrayService;

/// <summary>
///     Сервис управл
/// </summary>
internal sealed class AppTrayService : IAppTrayService, IEnableLogger
{
    private readonly IClassicDesktopStyleApplicationLifetime? _application;
    
    public AppTrayService()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime application)
        {
            _application = application;
        }
        else
        {
            this.Log().StructLogFatal("Can't get application instance");
        }
    }
    
    public void ShowWindow()
    {
        if (_application is null)
        {
            this.Log().StructLogFatal("Can't get application instance");
            return;
        }
        
        // Если загружен то новое не создаем
        if (_application.MainWindow is { IsLoaded: true })
        {
            return;
        }
        
        var newMainWindow = Locator.Current.GetService<MainWindow>()!;
        newMainWindow.DataContext = Locator.Current.GetService<MainWindowViewModel>();

        _application.MainWindow = newMainWindow;
        
        newMainWindow.WindowState = WindowState.Normal;
        newMainWindow.Show();
        newMainWindow.BringIntoView();
        newMainWindow.Focus();
    }

    public void CloseApp()
    {
        if (_application is null)
        {
            this.Log().StructLogFatal("Can't get application instance");
            return;
        }
        
        _application.Shutdown();
    }

    public void ChangeShutdownPolitic(ShutdownMode mode)
    {
        if (_application is null)
        {
            this.Log().StructLogFatal("Can't get application instance");
            return;
        }

        _application.ShutdownMode = mode;
    }
}