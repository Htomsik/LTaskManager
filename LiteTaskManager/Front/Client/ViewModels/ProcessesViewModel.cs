using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Client.Extensions;
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
   ///   Альетрнативная коллекция для деревьев
   /// </summary>
   [Reactive]
   public HierarchicalTreeDataGridSource<TaskProcess>? ItemsHierarch { get; set; }

   /// <summary>
   ///   Сервис обрабатывающий процессы
   /// </summary>
   public  IProcessService<TaskProcess> ProcessService { get; }
   
   /// <summary>
   ///   Сервис информации о приложении
   /// </summary>
   public IAppInfoService AppInfoService { get; }
   
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
   /// <param name="appInfoService"> Сервис информации о приложении</param>
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
      
      processService.ProcessesChanged += () =>
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
            .Subscribe(_ =>
            {
               if (ItemsHierarch is not null)
               {
                  ItemsHierarch.Items = Items!;
               }
               else
               {
                  ItemsHierarch = new HierarchicalTreeDataGridSource<TaskProcess>(Items!)
                  {
                     Columns =
                     {
                        new HierarchicalExpanderColumn<TaskProcess>(
                           new TextColumn<TaskProcess, string>
                              (Assets.Resources.ProcessName, x => x.ProcessName), 
                           x => x.Childs,
                           x => x.Childs.Count >0),
                        
                        new TextColumn<TaskProcess, double>("Id", x => x.ProcessId),
                        new TemplateColumn<TaskProcess>($"{Assets.Resources.ProcessCPUUsage} (%)", ResourcesExtension.ProcessCpuUsagePercentTemplate),
                        new TemplateColumn<TaskProcess>($"{Assets.Resources.ProcessRamUsage} (%)", ResourcesExtension.ProcessRamUsagePercentTemplate),
                        new TextColumn<TaskProcess, ProcessPriorityClass?>(Assets.Resources.ProcessPriority, x => x.PriorityClassCore),
                        new TextColumn<TaskProcess, string>(Assets.Resources.ProcessProductName, x => x.ProductName),
                        new TextColumn<TaskProcess, TimeSpan>(Assets.Resources.ProcessTotalProcessorTime, x => x.TotalProcessorTime),
                     }
                  };
               }
               
               if (ItemsHierarch.RowSelection != null)
               {
                  ItemsHierarch.RowSelection.SelectionChanged += (_, args) =>
                  {
                     ProcessService.CurrentProcess = args.SelectedItems[0];
                  };
               }
                  
            });
      
      this.RaisePropertyChanged(nameof(Items));
   }

   #endregion

   #region Methods

   /// <summary>
   ///   Фильтр по категориям
   /// </summary>
   private  Func<TaskProcess, bool> OnlySystemProcessFilterBuilder(ProcessCategory category)
   {
      if (category == ProcessCategory.NotSystems)
      {
         return entity => !_systemProcessLists.Any(elem => elem.Contains(entity.ProcessName,StringComparison.OrdinalIgnoreCase));
      }
      
      if (category == ProcessCategory.Systems)
      {
         return entity => _systemProcessLists.Any(elem => elem.Contains(entity.ProcessName,StringComparison.OrdinalIgnoreCase));
      }

      return _ => true;
   }

   #endregion
}
