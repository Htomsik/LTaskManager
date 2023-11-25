using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

namespace Client.Models;

/// <summary>
///     Модель конфигцрации приложения
/// </summary>
internal sealed class AppSettings : ReactiveValidationObject
{
    /// <summary>
    ///     Делей между обновлениями процессов
    /// </summary>
    public double ProcessUpdateTimeOut
    {
        get => _processUpdateTimeOut;
        set
        {
            if (value < 0)
            {
                value = 0;
            }
            
            this.RaiseAndSetIfChanged(ref _processUpdateTimeOut, value);
           
            if (value < 5)
            {
                _processUpdateTimeOut = 5;
            }

        }
    }

    private double _processUpdateTimeOut;

    /// <summary>
    ///     Текущая выбранная локализация
    /// </summary>
    [Reactive]
    public AppCulture Culture { get; set; }


    public AppSettings()
    {
        SetValidation();
    }

    /// <summary>
    ///     Устанавливает валидацию для текущей модели
    /// </summary>
    private void SetValidation()
    {
       this.ValidationRule(
            appSettings => appSettings.ProcessUpdateTimeOut, 
            value => value >= 5,
            "TimeOut must be more than 4 seconds");

    }
}