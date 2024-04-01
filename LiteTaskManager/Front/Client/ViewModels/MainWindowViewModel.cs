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
    ///     Статус - бар. Нижняя часть программы в которую выводится интерактивная информация пользователию
    /// </summary>
    [Reactive] public ViewModelBase? StatusBarViewModel { get; set; } 
    
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
        StatusBarViewModel statusBarViewModel, 
        AgreementViewModel agreementViewModel,
        ProcessesViewModel processesViewModel,
        IStore<AppSettings> appSettingsStore)
    {
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

        #region Commands exeptions logging
        
        Navigate.ThrownExceptions.Subscribe(x => this.Log().Error($"Execptions then processing {nameof(Navigate)} command:{x.Message}"));
        SaveAppSettings.ThrownExceptions.Subscribe(x => this.Log().Error($"Execptions then processing {nameof(SaveAppSettings)} command:{x.Message}"));
        GetAppSettings.ThrownExceptions.Subscribe(x => this.Log().Error($"Execptions then processing {nameof(SaveAppSettings)} command:{x.Message}"));

        #endregion

        #region Commands logging

        Navigate.Subscribe(_ => this.Log().Info($"Processing commnad {nameof(Navigate)}. Page {CurrentViewModel?.GetType()} is open"));

        #endregion

        #region VMD initialize

        CurrentViewModel = processesViewModel;
        StatusBarViewModel = statusBarViewModel;
        AgreementViewModel = agreementViewModel;

        #endregion
        
        MenuList = new ObservableCollection<MenuParamCommandItem>
        { 
            new MenuParamCommandItem("Processes", Navigate, typeof(ProcessesViewModel), MaterialIconKind.Memory),
        };
        
        AppSettings = appSettingsStore.CurrentValue;
        
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