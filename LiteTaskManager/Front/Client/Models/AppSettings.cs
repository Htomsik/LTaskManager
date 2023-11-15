using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Client.Models;

/// <summary>
///     Модель конфигцрации приложения
/// </summary>
internal sealed class AppSettings : ReactiveObject
{
    /// <summary>
    ///     Делей между обновлениями процессов
    /// TODO: когда будем внедрять валидацию, сделать проверку на то чтобы нельзя было обновлять меньше 2-3 секунд
    /// </summary>
    public double ProcessUpdateTimeOut 
    {
        get => _processUpdateTimeOut;
        set
        {
            if (value < 3)
                value = 3;
            
            _processUpdateTimeOut = value;
            this.RaisePropertyChanged(nameof(ProcessUpdateTimeOut));
        }
    }
    
    private double _processUpdateTimeOut = 5;
    
    /// <summary>
    ///     Текущая выбранная локализация
    /// </summary>
    [Reactive]
    public AppCulture Culture { get; set; }
}