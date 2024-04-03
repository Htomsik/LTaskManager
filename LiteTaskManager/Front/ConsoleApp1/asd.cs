using System.Diagnostics;

namespace ConsoleApp1;

public class GetCPUUsage
{
    static TimeSpan start;
    public static double CPUUsageTotal
    {
        get;
        private set;
    }

    public static double CPUUsageLastMinute
    {
        get;
        private set;
    }

    static TimeSpan oldCPUTime = new TimeSpan(0);
    static DateTime lastMonitorTime = DateTime.UtcNow;
    public static DateTime StartTime = DateTime.UtcNow;
    
    
    // Call it once everything is ready
   public static void OnStartup()
    {
        start = Process.GetProcessById(6692).TotalProcessorTime;
    }

    // Call this every 30 seconds
  public  static void CallCPU()
    {
        TimeSpan newCPUTime = Process.GetCurrentProcess().TotalProcessorTime - start;
        CPUUsageLastMinute = (newCPUTime - oldCPUTime).TotalSeconds / (Environment.ProcessorCount * DateTime.UtcNow.Subtract(lastMonitorTime).TotalSeconds);
        lastMonitorTime = DateTime.UtcNow;
        CPUUsageTotal = newCPUTime.TotalSeconds / (Environment.ProcessorCount * DateTime.UtcNow.Subtract(StartTime).TotalSeconds);
        oldCPUTime = newCPUTime;
    }
}