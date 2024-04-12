using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AppInfrastructure.Stores.DefaultStore;
using Client.Infrastructure.Logging;
using Client.Models;
using Client.Models.TaskProcess;
using Client.Models.TaskProcess.Base;
using Client.Services.Base;
using Client.Services.ComputerInfoService.Base;
using Splat;

namespace Client.Services;

/// <summary>
///     Сервис работы с процессами для винды
/// </summary>
internal sealed class WindowsProcessService : BaseProcessService<WindowsProcess>
{
    public WindowsProcessService(IStore<AppSettings> appSettingStore, IComputerInfoService computerInfoService) : base(appSettingStore, computerInfoService)
    {
    }
    
    protected override void UpdateProcessesCore()
    {
        // 1 итерация, собираем все процессы в один список
        var buffer = new ConcurrentDictionary<int, WindowsProcess>();
            
        Parallel.ForEach(Process.GetProcesses() ,process =>
        {
            var taskProcess = new WindowsProcess(process);
            
            buffer.TryAdd(taskProcess.ProcessId, taskProcess);
        });
        
        // 2 распределяем по родителям
        var childIdx = new ConcurrentBag<int>();
        
        Parallel.ForEach(buffer.Values.Where(taskProcess => taskProcess.ParentId != 0), taskProcess =>
        {
            var getParent = buffer.TryGetValue(taskProcess.ParentId, out var parentTaskProcess);
            
            if (!getParent)
            {
                try
                {
                    var parent = Process.GetProcessById(taskProcess.ParentId);
                    parentTaskProcess = new WindowsProcess(parent);
                    buffer.TryAdd(parentTaskProcess.ProcessId, parentTaskProcess);
                }
                catch
                {
                    this.Log().StructLogDebug("Can't get parent process");
                    return;
                }
            }
            
            if (parentTaskProcess == null) return;
            
            parentTaskProcess.Childs.Add(taskProcess);
            
            childIdx.Add(taskProcess.ProcessId);
        });
            
        // 3. Удаляем лишние
        foreach (var processId in childIdx)
        {
            buffer.TryRemove(processId, out var process);
        }
            
        Processes = new ObservableCollection<IProcess>(buffer.Values);
    }
}