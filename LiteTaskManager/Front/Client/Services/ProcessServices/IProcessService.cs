using System.Collections.ObjectModel;

namespace Client.Services;

public interface IProcessService<TProcess>
{
    public ObservableCollection<TProcess> Processes { get; }
    
    public TProcess CurrentProcess { get; set; }

    public void StopCurrentProcess();
    
    public void UpdateProcesses();
}