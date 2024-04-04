using System;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Client.Timers.Base;

/// <summary>
///     Основа для реактивных таймеров
/// </summary>
public class ReactiveTimer : ReactiveObject, IReactiveTimer
{
    #region Proeprties

    public int UpdateDelaySeconds 
    { 
        get => _updateDelaySeconds;
        set
        {
            if (value < 1 || value == _updateDelaySeconds)
            {
                return;
            }
            
            this.RaiseAndSetIfChanged(ref _updateDelaySeconds, value, nameof(_updateDelaySeconds));

            // Если запущен - перезапускаем
            if (Executing)
            {
                Start();
            }
        } 
    }

    private int _updateDelaySeconds = 1;
    
    public bool ReCreate { get; set; } = true;
    
    [Reactive]
    public bool Executing { get; set; }
    
    public event Action<long>? TimerChangedNotifier;

    #endregion

    #region Fields

    /// <summary>
    ///     Действие по окончанию таймера
    /// </summary>
    private readonly Action _actionOver;
    
    private IDisposable? _disposable;

    #endregion

    #region Constructors

    public ReactiveTimer(Action actionOver)
    {
        _actionOver = actionOver ?? throw new ArgumentNullException(nameof(actionOver));
    }

    #endregion
    
    #region Methods

    public void Start()
    {
        _disposable?.Dispose();
        
        _disposable = Observable
            .Timer(DateTimeOffset.Now, TimeSpan.FromSeconds(1))
            .Select(currentSeconds => UpdateDelaySeconds  - currentSeconds)
            .TakeWhile(currentSeconds => currentSeconds >= 0)
            .Subscribe(OnTimerChanged);

        Executing = true;
    }
    
    public void Stop()
    {
        _disposable?.Dispose();

        Executing = false;
    }
    
    private void OnTimerChanged(long currentSec)
    {
        TimerChangedNotifier?.Invoke(currentSec);
        
        if(currentSec != 0) return;
        
        _actionOver?.Invoke();

        if (ReCreate)
        {
            Start();
        }
    } 

    #endregion
}