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
    #region Constructors

    public UnixProcess(Process process) : base(process)
    {
    }
    
    public UnixProcess(IProcess process, int processId) : base(process, processId)
    {
        
    }
    #endregion

    protected override int GetParentId()
    {
        // TODO Доработать под линукс
        return 0;
    }

    protected override ICollection<ProcessAffinityCore> GetProcessorAffinity()
    {
        try
        {
            var affinity = new List<ProcessAffinityCore>();
            
            var chars = Convert.ToString(Process.ProcessorAffinity, 2).ToList();

            // TODO: Ненадежный метод, когда текущий процесс будет перенесен в отдельный класс
            // Переделать через получение количества ядер пк из wmi
            var deltaProcess = Environment.ProcessorCount - chars.Count;

            for (var i = 0; i < deltaProcess; i++)
            {
                chars.Insert(0,'0');
            }
            
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

    protected override bool ChangeAffinity()
    {
        if (HasExited || FakeProcess)
        {
            return false;
        }

        try
        {
            var affinity = ProcessorAffinityBackground.Select(x => x.Used ? "1" : "0");

            var binaryCode = string.Join("", affinity);

            var numberToHex = (IntPtr)Convert.ToUInt64(binaryCode, 2);

            // Не стоит лишний раз устанавливать приоритет 
            if (numberToHex == Process.ProcessorAffinity)
            {
                return false;
            }
        
            Process.ProcessorAffinity = numberToHex;
        }
        catch (Exception e)
        {
            this.Log().StructLogError($"Can't change affinity of {ProcessName} process", e.Message);
            return false;
        }
        
        this.Log().StructLogInfo($"Process {ProcessName} Affinity changed");
        
        return true;
    }

    protected override void ReCalCpuUsage()
    {
        // TODO Доработать под линукс
    }
}