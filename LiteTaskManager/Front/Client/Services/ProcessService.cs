using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace Client.Services;

internal class ProcessService : ReactiveObject, IProcessService<Process>
{
    [Reactive]
    public ObservableCollection<Process> Processes { get; set; }
    
    [Reactive]
    public Process? CurrentProcess { get; set; }
    
    public double UpdateTimerSeconds { get; set; } = 10;

    private IDisposable _processDisposable;
    
    public ProcessService()
    {
        UpdateProcesses();
    }

    private void SetSubscribes()
    {
        _processDisposable?.Dispose();
        
        _processDisposable = this.WhenAnyValue(x=> x.Processes)
            .Throttle(TimeSpan.FromSeconds(UpdateTimerSeconds))
            .Subscribe(x => UpdateProcesses());
    }
    
    public void StopCurrentProcess()
    {
        try
        {
            CurrentProcess.Kill();
        }
        catch (Exception e)
        {
            this.Log().Error($"Can't {nameof(StopCurrentProcess)} {nameof(CurrentProcess)}. {e.Message}");
            return;
        }
        
        this.Log().Warn($"Process {CurrentProcess.ProcessName} was killed");
    }

    
    public void UpdateProcesses()
    {
        try
        {
            Processes = new ObservableCollection<Process>(Process.GetProcesses());

            SetSubscribes();
        }
        catch (Exception e)
        {
            this.Log().Error($"Can't {nameof(UpdateProcesses)} {nameof(Processes)} . {e.Message}");
            return;
        }
        
        this.Log().Warn($"{nameof(UpdateProcesses)} {nameof(Processes)} sucess");
    }
}