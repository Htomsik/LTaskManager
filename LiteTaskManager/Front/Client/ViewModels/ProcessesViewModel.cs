using System;
using System.Reactive;
using Client.Models;
using Client.Services;
using ReactiveUI;
using Splat;

namespace Client.ViewModels;

internal sealed class ProcessesViewModel : BaseCollectionViewModel<TaskProcess>
{
   public  IProcessService<TaskProcess> ProcessService { get; init; }
   
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
   
   public ReactiveCommand<Unit, Unit> KillProcess { get; init; }
}
