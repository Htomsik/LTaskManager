using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Client.Infrastructure.Logging;
using Client.Models.TaskProcess.Base;
using Splat;

namespace Client.Models.TaskProcess;

[SuppressMessage("Interoperability", "CA1416:Проверка совместимости платформы")]
public class UnixProcess : BaseProcess
{
    public UnixProcess(Process process) : base(process)
    {
    }

    protected override int GetParentId()
    {
        // TODO Доработать под линукс
        return 0;
    }

    protected override ICollection<ProcessAffinityCore> GetProcessorAffinity()
    {
        try
        {
            var chars = Convert.ToString(Process.ProcessorAffinity, 2).ToList();
            
            var affinity = new ObservableCollection<ProcessAffinityCore>();
            
            for (var index = 0; index < chars.Count; index++)
            {
                affinity.Add(new ProcessAffinityCore(index, chars[index] == '1'));
            }
            
            return affinity;
        }
        catch (Exception e)
        {
            this.Log().StructLogDebug("Can't get affinity", e.Message);
            return new ObservableCollection<ProcessAffinityCore>();
        }
    }

    protected override void ReCalCpuUsage()
    {
        // TODO Доработать под линукс
    }
}