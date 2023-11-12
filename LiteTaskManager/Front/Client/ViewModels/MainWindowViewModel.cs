using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
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

    [Reactive] public ViewModelBase? CurrentViewModel { get; set; } 
    
    [Reactive] public ViewModelBase? StatusBarViewModel { get; set; } 
    
    public MainWindowViewModel(IStoreFileService<IStore<AppSettings>, AppSettings> appSettingsFileService, StatusBarViewModel statusBarViewModel)
    {
        _appSettingsFileService = appSettingsFileService;
        
        #region Commands Initialzie

        Navigate = ReactiveCommand.Create<Type>(
            type =>  CurrentViewModel = (Locator.Current.GetService(type) as ViewModelBase)!);
        
        Navigate.ThrownExceptions.Subscribe(x => this.Log().Error($"Execptions then processing {nameof(Navigate)} command:{x.Message}"));
        
        SaveAppSettings = ReactiveCommand.CreateFromTask(_appSettingsFileService.SetAsync);
        SaveAppSettings.ThrownExceptions.Subscribe(x => this.Log().Error($"Execptions then processing {nameof(SaveAppSettings)} command:{x.Message}"));

        GetAppSettings = ReactiveCommand.CreateFromTask(_appSettingsFileService.GetAsync);
        GetAppSettings.ThrownExceptions.Subscribe(x => this.Log().Error($"Execptions then processing {nameof(SaveAppSettings)} command:{x.Message}"));

        OpenAppSettings = ReactiveCommand.Create(() =>
        {
            CurrentViewModel = Locator.Current.GetService<AppSettingsViewModel>();
        });
        
        OpenAboutInfo = ReactiveCommand.Create(() =>
        {
            CurrentViewModel = Locator.Current.GetService<AppInfoViewModel>();
        });

        #endregion
        
        MenuList = new ObservableCollection<MenuParamCommandItem>
        { 
            new MenuParamCommandItem("Processes", Navigate, typeof(ProcessesViewModel), MaterialIconKind.Memory),
        };
        
        CurrentViewModel = Locator.Current.GetService<ProcessesViewModel>();

        StatusBarViewModel = statusBarViewModel;
    }
    
    public ReactiveCommand<Type,Unit> Navigate { get; init; }
    
    public ReactiveCommand<Unit, Unit> OpenAppSettings { get; set; }
    
    public ReactiveCommand<Unit, Unit> OpenAboutInfo { get; set; }
    
    public ReactiveCommand<Unit, bool> SaveAppSettings { get; set; }
    
    public ReactiveCommand<Unit, bool> GetAppSettings { get; set; }
}