using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using AppInfrastructure.Stores.DefaultStore;
using Client.Models;
using Client.Models.TaskProcess;
using Client.Models.TaskProcess.Base;
using Client.Services.Base;
using Client.Services.ComputerInfoService.Base;

namespace Client.Services;

internal sealed class UnixProcessService : BaseProcessService<UnixProcess>  
{
    protected override void UpdateProcessesCore()
    {
        var buffer = new ConcurrentBag<UnixProcess>();

        Parallel.ForEach(Process.GetProcesses(), process =>
        {
            var taskProcess = new UnixProcess(process);

            buffer.Add(taskProcess);
        });
        
        Processes = new ObservableCollection<IProcess>(buffer);
    }

    public UnixProcessService(IStore<AppSettings> appSettingStore, IComputerInfoService computerInfoService) : base(appSettingStore, computerInfoService)
    {
    }
}