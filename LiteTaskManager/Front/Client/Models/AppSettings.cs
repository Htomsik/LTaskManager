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
    ///     Ручной режим обновления и перерасчте
    /// </summary>
    [Reactive]
    public bool ManualMode { get; set; }
    
    /// <summary>
    ///     Принятие соглашения (отказа от ответственности)
    /// </summary>
    [Reactive]
    public bool Agreement { get; set; }
    
    #region ProcessUpdateTimeOut

    /// <summary>
    ///     Делей между загрузкой новых процессов
    /// </summary>
    public int ProcessUpdateTimeOut
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

    private int _processUpdateTimeOut = 5;

    #endregion
    
    #region ProcessReCalcTimeOut

    /// <summary>
    ///     Делей между обновлением данных процессов
    /// </summary>
    public int ProcessReCalcTimeOut
    {
        get => _processReCalcTimeOut;
        set
        {
            if (value < 0)
            {
                value = 0;
            }
            
            this.RaiseAndSetIfChanged(ref _processReCalcTimeOut, value);
           
            if (value < 1)
            {
                _processReCalcTimeOut = 1;
            }

        }
    }

    private int _processReCalcTimeOut = 1;

    #endregion
    
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
            value => value > 4,
            "TimeOut must be more than 4 seconds");
       
       this.ValidationRule(
           appSettings => appSettings._processReCalcTimeOut, 
           value => value > 1,
           "TimeOut must be more than 1 second");
    }
}