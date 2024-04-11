using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
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
        new Action(() =>
        {
            // 1 итерация, собираем все процессы в один список
            var buffer = new Dictionary<int, WindowsProcess>();
            foreach (var process in Process.GetProcesses())
            {
                var taskProcess = new WindowsProcess(process);
                
                buffer.Add(taskProcess.ProcessId,  taskProcess);
            }

            var childsIdx = new List<int>();
            // 2 итерация, распределяем процессы по родителям
            foreach (var taskProcess in buffer.Values.Where(taskProcess => taskProcess.ParentId != 0))
            {
                var getParent =  buffer.TryGetValue(taskProcess.ParentId, out var parentTaskProcess);
                
                if (!getParent)
                {
                    try
                    {
                        var parent =   Process.GetProcessById(taskProcess.ParentId);
                        parentTaskProcess = new WindowsProcess(parent);
                        buffer.Add(parentTaskProcess.ProcessId, parentTaskProcess);
                    }
                    catch
                    {
                        this.Log().StructLogDebug("Can't get parent process");
                        continue;
                    }
                }

                if (parentTaskProcess == null) continue;
                
                parentTaskProcess?.Childs.Add(taskProcess);

                childsIdx.Add(taskProcess.ProcessId);
            }
            
            // 3. Удаляем лишние
            foreach (var processId in childsIdx)
            {
                buffer.Remove(processId);
            }
            
            Processes = new ObservableCollection<IProcess>(buffer.Values);
            
        }).TimeLog(this.Log());
    }
}