using System;
using System.Reactive;
using Client.Models;
using Client.Services;
using ReactiveUI;
using Splat;

namespace Client.ViewModels;

/// <summary>
///   Вьюмодель процессов
/// </summary>
internal sealed class ProcessesViewModel : BaseCollectionViewModel<TaskProcess>
{
   /// <summary>
   ///   Сервис обрабатывающий процессы
   /// </summary>
   public  IProcessService<TaskProcess> ProcessService { get; init; }
   
   /// <param name="processService">Сервис обрабатывающий процессы</param>
   public ProcessesViewModel(IProcessService<TaskProcess> processService) : base()
   {
      ProcessService = processService;
      
      KillProcess = ReactiveCommand.Create(processService.StopCurrentProcess);
      KillProcess.ThrownExceptions.Subscribe(x => this.Log().Error($"Execptions then processing {nameof(KillProcess)} command:{x.Message}"));
      
      processService.ProcessSubscriptionsChanged += () =>
      {
         SetItemsSubscriptions(processService.Processes);
      };
      
      SetFiltersSubscriptions();
      SetItemsSubscriptions(processService.Processes);
   }
   
   /// <summary>
   ///   Завершение процесса
   /// </summary>
   public ReactiveCommand<Unit, Unit> KillProcess { get; init; }
}
