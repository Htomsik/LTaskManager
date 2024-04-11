using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using AppInfrastructure.Stores.DefaultStore;
using Client.Infrastructure.Logging;
using Client.Models;
using Client.Models.TaskProcess;
using Client.Models.TaskProcess.Base;
using Client.Services.Base;
using Client.Services.ComputerInfoService;
using Splat;

namespace Client.Services;

internal sealed class UnixProcessService : BaseProcessService<UnixProcess>  
{
    protected override void UpdateProcessesCore()
    {
        new Action(() =>
        {
            var buffer = new HashSet<UnixProcess>();
            foreach (var process in Process.GetProcesses())
            {
                var taskProcess = new UnixProcess(process);
                buffer.Add(taskProcess);
            }
            
            Processes = new ObservableCollection<IProcess>(buffer);
            
        }).TimeLog(this.Log());
    }

    public UnixProcessService(IStore<AppSettings> appSettingStore, IComputerInfoService computerInfoService) : base(appSettingStore, computerInfoService)
    {
    }
}