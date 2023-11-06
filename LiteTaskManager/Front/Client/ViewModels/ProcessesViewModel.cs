using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace Client.ViewModels;

public class ProcessesViewModel : ViewModelBase
{
   [Reactive]
   public ObservableCollection<Process> Processes { get; set; }
   
   public ProcessesViewModel()
   {
      Processes = new ObservableCollection<Process>(Process.GetProcesses());
      
      KillProcess = ReactiveCommand.Create<Process>(
         p =>
         {
            p.Kill();
            this.Log().Warn($"Process {p.ProcessName} was killed");
         });

      KillProcess.ThrownExceptions.Subscribe(x => this.Log().Error($"Execptions then processing {nameof(KillProcess)} command:{x.Message}"));
   }
   
   public IReactiveCommand KillProcess { get; init; }
}
