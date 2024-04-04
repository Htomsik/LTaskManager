using System;

namespace Client.Timers.Base;

/// <summary>
///     Таймер 
/// </summary>
public interface IReactiveTimer
{
    /// <summary>
    ///     Делей между обновлениями
    /// </summary>
    public int UpdateDelaySeconds { get; set; }
    
    /// <summary>
    ///     Пересоздавать ли таймер заново при истечении времени
    /// </summary>
    public bool ReCreate { get; set; }
    
    /// <summary>
    ///     Уведомлятор о том сколько времени осталось
    /// </summary>
    public event Action<long>? TimerChangedNotifier;
    
    /// <summary>
    ///     Стартовать таймера 
    /// </summary>
    public void Start();
    
    /// <summary>
    ///     Остановка таймера
    /// </summary>
    public void Stop();
}