using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Client.Infrastructure.Logging;
using Client.Models;
using Client.Services;
using Client.Services.AppInfoService;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace Client.ViewModels;

/// <summary>
///   Вьюмодель процессов
/// </summary>
internal sealed class ProcessesViewModel : BaseCollectionViewModel<TaskProcess>
{
   #region Properties

   /// <summary>
   ///   Сервис обрабатывающий процессы
   /// </summary>
   public  IProcessService<TaskProcess> ProcessService { get; init; }
   
   /// <summary>
   ///   Сервис информации о приложении
   /// </summary>
   public IAppInfoService AppInfoService { get; init; }
   
   /// <summary>
   ///     Отображаемые категории процессов
   /// </summary>
   [Reactive]
   public ProcessCategory ShowedProcessCategory { get; set; }
   
   #endregion

   #region Fields

   /// <summary>
   ///     Наблюдатель за свойством поиска
   /// </summary>
   private IObservable<Func<TaskProcess, bool>> _onlySystemProcessFilter = null!;

   /// <summary>
   ///   Список системных процессов
   /// TODO: загружать их из файла
   /// </summary>
   private List<string> _systemProcessLists = new List<string>()
   {
      "idle",
      "explorer",
      "ntoskrnl",
      "werfault",
      "backgroundtaskhost",
      "backgroundtransferhost",
      "winlogon",
      "wininit",
      "csrss",
      "lsass",
      "smss",
      "services",
      "taskeng",
      "taskhost",
      "dwm",
      "conhost",
      "svchost",
      "sihost",
      "system",
      "vmms",
      "vmcomputer",
      "Secure system",
      "Registry",
      "fontdrvhost",
      "wudfhost",
      "spoolsv",
      "Lsalso",
      "wlanext"
   };

   #endregion

   #region Constructor

   /// <param name="processService">Сервис обрабатывающий процессы</param>
   public ProcessesViewModel(IProcessService<TaskProcess> processService, IAppInfoService appInfoService) : base()
   {
      AppInfoService = appInfoService;
      ProcessService = processService;
      
      KillProcess = ReactiveCommand.Create(processService.StopCurrentProcess);
      
      #region Command logging

      KillProcess.ThrownExceptions.Subscribe(e =>
         this.Log().StructLogError($"Processing", e.Message, nameof(KillProcess)));
        
      KillProcess.Subscribe(_ => this.Log().StructLogInfo($"Processing", nameof(KillProcess)));

      #endregion
      
      processService.ProcessSubscriptionsChanged += () =>
      {
         SetItemsSubscriptions(processService.Processes);
      };
      
      SetFiltersSubscriptions();
      SetItemsSubscriptions(processService.Processes);
   }

   #endregion
   
   /// <summary>
   ///   Завершение процесса
   /// </summary>
   public ReactiveCommand<Unit, Unit> KillProcess { get; init; }

   #region Subscriptions

   protected override void SetFiltersSubscriptions()
   {
      base.SetFiltersSubscriptions();
      
      _onlySystemProcessFilter = null!;
        
      _onlySystemProcessFilter =
         this.WhenValueChanged(x => x.ShowedProcessCategory)
            .Throttle(TimeSpan.FromMicroseconds(250))
            .Select(OnlySystemProcessFilterBuilder);
   }
   
   protected override void SetItemsSubscriptions(ObservableCollection<TaskProcess> inputData)
   {
      // Предварительная утилизация старой подписки
      ItemsSubscriptions?.Dispose();
        
      ItemsSubscriptions =
         inputData
            .ToObservableChangeSet()
            .Filter(SearchFilter)
            .Filter(_onlySystemProcessFilter)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out SelectedItems)
            .DisposeMany()
            .Subscribe();
        
      this.RaisePropertyChanged(nameof(Items));
   }

   #endregion

   #region Methods

   /// <summary>
   ///   Фильтр по категориям
   /// </summary>
   private  Func<TaskProcess, bool> OnlySystemProcessFilterBuilder(ProcessCategory category)
   {
      if (category == ProcessCategory.notSystems)
      {
         return entity => !_systemProcessLists.Any(elem => elem.Contains(entity.ProcessName,StringComparison.OrdinalIgnoreCase));
      }
      
      if (category == ProcessCategory.systems)
      {
         return entity => _systemProcessLists.Any(elem => elem.Contains(entity.ProcessName,StringComparison.OrdinalIgnoreCase));
      }

      return _ => true;
   }

   #endregion
}
