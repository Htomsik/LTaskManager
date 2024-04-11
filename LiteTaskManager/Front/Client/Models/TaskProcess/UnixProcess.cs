using System.Diagnostics;
using Client.Models.TaskProcess.Base;

namespace Client.Models.TaskProcess;

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

    protected override void ReCalCpuUsage()
    {
        // TODO Доработать под линукс
    }
}