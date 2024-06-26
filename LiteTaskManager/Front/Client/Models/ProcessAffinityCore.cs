﻿using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Client.Models;

/// <summary>
///     Ядро приоритета процесса
/// </summary>
public class ProcessAffinityCore : ReactiveObject
{
    /// <summary>
    ///     Номер ядра
    /// </summary>
    public int Number { get; set; }
    
    /// <summary>
    ///     Используется ли
    /// </summary>
    [Reactive]
    public bool Used { get; set; } = false;
    
    #region Constructors

    public ProcessAffinityCore(int number, bool used)
    {
        Number = number;
        Used = used;
    }

    #endregion
}