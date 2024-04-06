using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Client.Infrastructure.DI;
using Client.Infrastructure.Logging;
using Client.ViewModels;
using Client.Views;
using ReactiveUI;
using Splat;

namespace Client;

public partial class App : Application, IEnableLogger
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        AppConfiguration.Configure();
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime)
        {
            RxApp.DefaultExceptionHandler = Locator.Current.GetService<IObserver<Exception>>()!;
        }
        
        base.OnFrameworkInitializationCompleted();

        PostStartupEvents();
    }
    
    private void PostStartupEvents()
    {
        if (ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop) return;
        
        new Action(() =>
        {
            try
            {
                desktop.MainWindow = Locator.Current.GetService<MainWindow>();
                desktop.MainWindow!.DataContext = Locator.Current.GetService<MainWindowViewModel>();
            }
            catch (Exception e)
            {
               this.Log().StructLogError("Can't initialize main window",e.Message);
                throw;
            }
            
        }).TimeLog(this.Log());
    }
}