using System;
using System.Diagnostics;
using Client.Services;
using ReactiveUI;
using Splat;

namespace Client.ViewModels;

public class ProcessesViewModel : ViewModelBase
{
   public IProcessService<Process> ProcessService { get; }

   public ProcessesViewModel(IProcessService<Process> processService)
   {
      ProcessService = processService;
      
      KillProcess = ReactiveCommand.Create(processService.StopCurrentProcess);

      KillProcess.ThrownExceptions.Subscribe(x => this.Log().Error($"Execptions then processing {nameof(KillProcess)} command:{x.Message}"));
   }
   
   
   public IReactiveCommand KillProcess { get; init; }
}
