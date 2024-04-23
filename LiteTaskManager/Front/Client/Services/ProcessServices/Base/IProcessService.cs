using System;
using System.Collections.ObjectModel;
using Client.Models.TaskProcess.Base;
using Client.Timers.Base;

namespace Client.Services.Base;

/// <summary>
///     Сервис обработки windows процессов
/// </summary>
/// <typeparam name="TProcess"></typeparam>
public interface IProcessService
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
    public ObservableCollection<IProcess> Processes { get; }
    
    /// <summary>
    ///     Текущий выбранный процесс
    /// </summary>
    public IProcess CurrentProcess { get; set; }
    
    /// <summary>
    ///     Остановка текущего процесса
    /// </summary>
    public void StopCurrentProcess();
    
    /// <summary>
    ///     Подгрузка новых процессов
    /// </summary>
    public void UpdateProcesses(bool alsoRefresh = true, bool setSubscriptions = true);

    /// <summary>
    ///     Обновление существующих процессов
    /// </summary>
    public void RefreshProcess();
}