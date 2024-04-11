using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Client.Extensions;
using Client.Infrastructure.Logging;
using Client.Models.TaskProcess.Base;
using Splat;

namespace Client.Models.TaskProcess;

/// <summary>
///     Windows процессы
/// </summary>
[SuppressMessage("Interoperability", "CA1416:Проверка совместимости платформы")]
public sealed class WindowsProcess : BaseProcess
{
    #region Fields

    /// <summary>
    ///     Счетчик производительности
    /// </summary>
    private readonly PerformanceCounter? _performanceCounterCpuUsage;
    
    /// <summary>
    ///     Счетчик производительности для получения parentId
    /// </summary>
    private readonly PerformanceCounter? _performanceCounterParentId;
    
    /// <summary>
    ///     Выполнен ли первый проход _performanceCounterCpuUsage
    /// </summary>
    private bool _perfRefreshed;

    #endregion
    
    public WindowsProcess(Process process) : base(process)
    {
        try
        {
            _performanceCounterCpuUsage = new PerformanceCounter(PerfCounterExtension.ProcessCategory, PerfCounterExtension.ProcessCpuUsageCounter, ProcessName, true);
            _performanceCounterParentId = new PerformanceCounter(PerfCounterExtension.ProcessCategory, PerfCounterExtension.ProcessParentId, ProcessName, true);
        }
        catch (Exception e)
        {
            this.Log().StructLogDebug($"Can't create {_performanceCounterCpuUsage}", e.Message);
        }
        
    }

    #region Methods

    #region Operations

    protected override int GetParentId()
    {
        if (_performanceCounterParentId is null)
        {
            this.Log().StructLogDebug($"{nameof(_performanceCounterParentId)} not initialized");
            return 0;
        }
        
        return (int)_performanceCounterParentId.NextValue();
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

    #endregion

    #region Recalc methods

    protected override void ReCalCpuUsage()
    {
        if (_performanceCounterCpuUsage is null)
        {
            this.Log().StructLogDebug($"{nameof(_perfRefreshed)} not initialized");
            return;
        }
        
        if (!_perfRefreshed)
        {
            _perfRefreshed = true;
            _performanceCounterCpuUsage.NextValue();
        }
        
        try
        {
            double cpuUsage; 
            
            // TODO не спраишивайте меня почему так, я сам не знаю
            if (ProcessName == ProcessExtension.ProcessIdle)
            {
                cpuUsage = _performanceCounterCpuUsage.NextValue() / 10000;
            }
            else
            {
                cpuUsage = _performanceCounterCpuUsage.NextValue() / 100;
            }

            CpuUsagePercent = double.Round(cpuUsage * 100, 2);
        }
        catch (Exception e)
        {
            this.Log().StructLogDebug(
                $"Don't have access to {PerfCounterExtension.ProcessCpuUsageCounter} {ProcessName}", e.Message);
        }
    }

    #endregion
    
    #endregion
}