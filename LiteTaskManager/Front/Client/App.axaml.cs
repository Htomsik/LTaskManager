using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Client.Infrastructure.DI;
using Client.ViewModels;
using Client.Views;
using ReactiveUI;
using Splat;

namespace Client;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        AppConfiguration.Configure();
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            RxApp.DefaultExceptionHandler = Locator.Current.GetService<IObserver<Exception>>();
        }
        
        base.OnFrameworkInitializationCompleted();

        PostStartupEvents();
    }
    
    private void PostStartupEvents()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = Locator.Current.GetService<MainWindow>();
            desktop.MainWindow.DataContext = Locator.Current.GetService<MainWindowViewModel>();
        }
    }
}