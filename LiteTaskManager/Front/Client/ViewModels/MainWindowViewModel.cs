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
        IStore<AppSettings> appSettingsStore)
    {
        #region Commands Initialzie

        Navigate = ReactiveCommand.Create<Type>(
            type =>  CurrentViewModel = (Locator.Current.GetService(type) as ViewModelBase)!);
        
        Navigate.ThrownExceptions.Subscribe(x => this.Log().Error($"Execptions then processing {nameof(Navigate)} command:{x.Message}"));
        
        SaveAppSettings = ReactiveCommand.CreateFromTask(appSettingsFileService.SetAsync);
        SaveAppSettings.ThrownExceptions.Subscribe(x => this.Log().Error($"Execptions then processing {nameof(SaveAppSettings)} command:{x.Message}"));

        GetAppSettings = ReactiveCommand.CreateFromTask(appSettingsFileService.GetAsync);
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

        AgreementViewModel = agreementViewModel;

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