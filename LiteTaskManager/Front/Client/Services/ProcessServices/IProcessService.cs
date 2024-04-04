using System;
using System.Collections.ObjectModel;
using Client.Timers.Base;

namespace Client.Services;

/// <summary>
///     Сервис обработки windows процессов
/// </summary>
/// <typeparam name="TProcess"></typeparam>
public interface IProcessService<TProcess>
{
    /// <summary>
    ///     Тааймер пересоздания процессов
    /// </summary>
    public IReactiveTimer UpdateTimer { get; }
    
    /// <summary>
    ///     Таймер обновления данных процессов
    /// </summary>
    public IReactiveTimer RefreshTimer { get;  }
    
    /// <summary>
    ///     Уведомитель о том что требуется перепривязать подписки
    /// <remarks>Отрабатывает в случае подгрузки новых процессов</remarks>
    /// </summary>
    public event Action ProcessesChanged;
    
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

    /// <summary>
    ///     Обновление существующих процессов
    /// </summary>
    public void RefreshProcess();
}