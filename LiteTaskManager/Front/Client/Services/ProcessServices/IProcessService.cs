using System;
using System.Collections.ObjectModel;

namespace Client.Services;

/// <summary>
///     Сервис обработки windows процессов
/// </summary>
/// <typeparam name="TProcess"></typeparam>
public interface IProcessService<TProcess>
{
    /// <summary>
    ///     Уведомитель об оставщемся времени до обновления процессов
    /// </summary>
    public event Action<double> UpdateTimerChangeNotifier;

    /// <summary>
    ///     Уведомитель о том что требуется перепривязать подписки
    /// <remarks>Отрабатывает в случае подгрузки новых процессов</remarks>
    /// </summary>
    public event Action ProcessesChanged;
    
    /// <summary>
    ///         Делей между обновлениями списка процессов
    /// </summary>
    public double UpdateTimerSeconds { get;}
    
    /// <summary>
    ///         Делеай между перерасчетом данных процессов
    /// </summary>
    public double ReCalcTimerSeconds { get;}
    
    /// <summary>
    ///      Загруженные процессы
    /// </summary>
    public ObservableCollection<TProcess> Processes { get; }
    
    /// <summary>
    ///     Текущий выбранный процесс
    /// </summary>
    public TProcess CurrentProcess { get; set; }
    
    /// <summary>
    ///     Остановка текущего процесса
    /// </summary>
    public void StopCurrentProcess();
    
    /// <summary>
    ///     Подгрузка новых процессов
    /// </summary>
    public void UpdateProcesses();
}