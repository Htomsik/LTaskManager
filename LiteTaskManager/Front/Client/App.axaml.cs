using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Client.Infrastructure.DI;
using Client.ViewModels;
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
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            RxApp.DefaultExceptionHandler = Locator.Current.GetService<IObserver<Exception>>()!;
            
            DataContext = Locator.Current.GetService<AppViewModel>();
        }
        
        base.OnFrameworkInitializationCompleted();
    }
}