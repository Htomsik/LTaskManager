using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using AppInfrastructure.Services.FileService;
using AppInfrastructure.Stores.DefaultStore;
using Client.Infrastructure.Logging;
using Client.Models;
using Material.Icons;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace Client.ViewModels;

/// <summary>
///     Главное окно
/// </summary>
internal sealed class MainWindowViewModel : ViewModelBase
{
    #region Properties

    /// <summary>
    ///     Текущая выбранная страница
    /// </summary>
    [Reactive] public ViewModelBase? CurrentViewModel { get; set; } 
    
    /// <summary>
    ///     Нижняя плашка с интерактивной информацией
    /// </summary>
    [Reactive] public ViewModelBase? ProcessStatusViewModel { get; set; } 
    
    /// <summary>
    ///     Нижняя плашка с уведомлениями
    /// </summary>
    [Reactive] public ViewModelBase? NotificationBarViewModel { get; set; } 
    
    /// <summary>
    /// Соглашение, которое не пускает в приложение, если его не принять
    /// </summary>
    [Reactive] public ViewModelBase? AgreementViewModel { get; set; }
    
    /// <summary>
    /// Настройки приложения
    /// </summary>
    [Reactive] public AppSettings AppSettings { get; set; }
    
    /// <summary>
    ///     Коллекцию кнопок для перехода на другую страницу
    /// </summary>
    public IEnumerable<MenuParamCommandItem> MenuList { get; set; }

    #endregion

    #region Constructors

    public MainWindowViewModel(IStoreFileService<IStore<AppSettings>, AppSettings> appSettingsFileService,
        AgreementViewModel agreementViewModel,
        ProcessesViewModel processesViewModel,
        ProcessStatusViewModel  processStatusViewModel,
        NotificationBarViewModel notificationBarViewModel,
        IStore<AppSettings> appSettingsStore)
    {
        AppSettings = appSettingsStore.CurrentValue;
        NotificationBarViewModel = notificationBarViewModel;
        ProcessStatusViewModel = processStatusViewModel;
        
        #region Commands Initialzie

        Navigate = ReactiveCommand.Create<Type>(
            type =>  CurrentViewModel = (Locator.Current.GetService(type) as ViewModelBase)!);
        
        SaveAppSettings = ReactiveCommand.CreateFromTask(appSettingsFileService.SetAsync);
        
        GetAppSettings = ReactiveCommand.CreateFromTask(appSettingsFileService.GetAsync);
        
        OpenAppSettings = ReactiveCommand.Create(() =>
        {
            CurrentViewModel = Locator.Current.GetService<AppSettingsViewModel>();
        });
        
        OpenAboutInfo = ReactiveCommand.Create(() =>
        { 
            CurrentViewModel = Locator.Current.GetService<AppInfoViewModel>();
        });

        #endregion

        #region  logging
        
        Navigate.Subscribe(_ => this.Log().StructLogInfo($"Processing", nameof(Navigate)));
        SaveAppSettings.Subscribe(_ => this.Log().StructLogInfo($"Processing", nameof(SaveAppSettings)));
        GetAppSettings.Subscribe(_ => this.Log().StructLogInfo($"Processing", nameof(GetAppSettings)));
        OpenAppSettings.Subscribe(_ => this.Log().StructLogInfo($"Processing", nameof(OpenAppSettings)));
        OpenAboutInfo.Subscribe(_ => this.Log().StructLogInfo($"Processing", nameof(OpenAboutInfo)));
        
        Navigate.ThrownExceptions.Subscribe(e =>
            this.Log().StructLogError($"Processing", e.Message, nameof(Navigate)));
        
        SaveAppSettings.ThrownExceptions.Subscribe(e =>
            this.Log().StructLogError($"Processing", e.Message, nameof(SaveAppSettings)));
        
        GetAppSettings.ThrownExceptions.Subscribe(e =>
            this.Log().StructLogError($"Processing", e.Message, nameof(GetAppSettings)));
        
        OpenAppSettings.ThrownExceptions.Subscribe(e =>
            this.Log().StructLogError($"Processing", e.Message, nameof(OpenAppSettings)));
        
        OpenAboutInfo.ThrownExceptions.Subscribe(e =>
            this.Log().StructLogError($"Processing", e.Message, nameof(OpenAboutInfo)));
        
        #endregion
        
        #region VMD initialize

        CurrentViewModel = processesViewModel;
        AgreementViewModel = agreementViewModel;

        #endregion
        
        MenuList = new ObservableCollection<MenuParamCommandItem>
        { 
            new MenuParamCommandItem("Processes", Navigate, typeof(ProcessesViewModel), MaterialIconKind.Memory),
        };
        
        appSettingsStore.CurrentValueChangedNotifier += ()=>
        {
            AppSettings = appSettingsStore.CurrentValue;
        };
    }

    #endregion

    #region Commands

    /// <summary>
    ///     Навгация между страницами
    /// </summary>
    public ReactiveCommand<Type,Unit> Navigate { get; init; }
    
    /// <summary>
    ///     Открытие страницы настроек
    /// <remarks> Переключение текущей вьюмодели страницы на вьюмодель настроек</remarks>
    /// </summary>
    public ReactiveCommand<Unit, Unit> OpenAppSettings { get; set; }
    
    /// <summary>
    ///     Открытие страницы информации о приложении
    /// <remarks> Переключение Текущей вьюмодели страницы на вьюмодель информации о приложении</remarks>
    /// </summary>
    public ReactiveCommand<Unit, Unit> OpenAboutInfo { get; set; }
    
    /// <summary>
    ///     Сохранение настроек приложения в файл
    /// </summary>
    public ReactiveCommand<Unit, bool> SaveAppSettings { get; set; }
    
    /// <summary>
    ///     Получение настроек приложения в файл
    /// </summary>
    public ReactiveCommand<Unit, bool> GetAppSettings { get; set; }

    #endregion
}