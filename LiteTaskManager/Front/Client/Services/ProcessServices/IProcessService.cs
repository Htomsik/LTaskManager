using System;
using System.Collections.ObjectModel;

namespace Client.Services;

public interface IProcessService<TProcess>
{
    /// <summary>
    ///     Уведомитель об оставщемся времени до обновления процессов
    /// </summary>
    public event Action<double> UpdateTimerChangeNotifier;
    
    /// <summary>
    ///         Делей между обновлениями списка процессов
    /// </summary>
    public double UpdateTimerSeconds { get; set; }
    
    public ObservableCollection<TProcess> Processes { get; }
    
    public TProcess CurrentProcess { get; set; }

    public void StopCurrentProcess();
    
    public void UpdateProcesses();
}