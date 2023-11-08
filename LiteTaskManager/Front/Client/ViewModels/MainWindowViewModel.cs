using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AppInfrastructure.Services.FileService;
using AppInfrastructure.Stores.DefaultStore;
using Client.Models;
using Material.Icons;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace Client.ViewModels;

internal sealed class MainWindowViewModel : ViewModelBase
{
    private readonly IStoreFileService<IStore<AppSettings>, AppSettings> _appSettingsFileService;
    public IEnumerable<MenuParamCommandItem> MenuList { get; set; }

    [Reactive] 
    public ViewModelBase CurrentViewModel { get; set; }
    
    public MainWindowViewModel(IStoreFileService<IStore<AppSettings>, AppSettings> appSettingsFileService)
    {
        _appSettingsFileService = appSettingsFileService;
        
        Navigate = ReactiveCommand.Create<Type>(
            type =>
            {
                CurrentViewModel = (Locator.Current.GetService(type) as ViewModelBase)!;
            });

        Navigate.ThrownExceptions.Subscribe(x => this.Log().Error($"Execptions then processing {nameof(Navigate)} command:{x.Message}"));
        
        SaveAppSettings = ReactiveCommand.Create(_appSettingsFileService.SetAsync);
        SaveAppSettings.ThrownExceptions.Subscribe(x => this.Log().Error($"Execptions then processing {nameof(SaveAppSettings)} command:{x.Message}"));

        GetAppSettings = ReactiveCommand.Create(_appSettingsFileService.GetAsync);
        GetAppSettings.ThrownExceptions.Subscribe(x => this.Log().Error($"Execptions then processing {nameof(SaveAppSettings)} command:{x.Message}"));

        OpenAppSettings =
            ReactiveCommand.Create(() => CurrentViewModel = Locator.Current.GetService<AppSettingsViewModel>()!);
        
        MenuList = new ObservableCollection<MenuParamCommandItem>
        { 
            new MenuParamCommandItem("Processes", Navigate, typeof(ProcessesViewModel), MaterialIconKind.Memory),
        };
        
        CurrentViewModel = Locator.Current.GetService<ProcessesViewModel>();
    }
    
    public IReactiveCommand Navigate { get; init; }
    
    public IReactiveCommand OpenAppSettings { get; set; }
    
    public IReactiveCommand SaveAppSettings { get; set; }
    
    public IReactiveCommand GetAppSettings { get; set; }
}