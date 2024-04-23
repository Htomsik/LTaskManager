using System;
using System.Reactive;
using AppInfrastructure.Services.FileService;
using AppInfrastructure.Stores.DefaultStore;
using Client.Infrastructure.Logging;
using Client.Models;
using Client.Services.AppTrayService;
using ReactiveUI;
using Splat;

namespace Client.ViewModels;

/// <summary>
///     Вьюмодель App
/// <remarks>Нужен для работы с треем</remarks>
/// </summary>
internal sealed class AppViewModel : ViewModelBase
{
    public AppViewModel(IAppTrayService appTrayService, 
        IStoreFileService<IStore<AppSettings>, AppSettings> appSettingsFileService)
    {
        new Action(() =>
        { 
            appSettingsFileService.GetAsync();
        }).TimeLog(this.Log(), $"{nameof(appSettingsFileService)}:{nameof(appSettingsFileService.GetAsync)}");
        
        Show = ReactiveCommand.Create(appTrayService.ShowWindow);
        Close = ReactiveCommand.Create(appTrayService.CloseApp);

        #region Commands logging
        
        Show.Subscribe(_ => this.Log().StructLogInfo($"Processing", nameof(Show)));

        Show.ThrownExceptions.Subscribe(e =>
            this.Log().StructLogError($"Processing", e.Message, nameof(Show)));

        Close.Subscribe(_ => this.Log().StructLogInfo($"Processing", nameof(Close)));

        Close.ThrownExceptions.Subscribe(e =>
            this.Log().StructLogError($"Processing", e.Message, nameof(Close)));
        
        #endregion
        
        appTrayService.ShowWindow();
    }
    
    public ReactiveCommand<Unit, Unit> Show { get; set; }
    
    public ReactiveCommand<Unit, Unit> Close { get; set; }
}