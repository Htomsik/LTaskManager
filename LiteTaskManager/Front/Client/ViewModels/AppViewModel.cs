﻿using System;
using System.Reactive;
using Client.Infrastructure.Logging;
using Client.Services.AppTrayService;
using ReactiveUI;
using Splat;

namespace Client.ViewModels;

/// <summary>
///     Вьюмодель App
/// <remarks>Нужен для работы с треем</remarks>
/// </summary>
internal sealed class AppViewModel : ViewModelBase
{
    public AppViewModel(IAppTrayService appTrayService)
    {
        Show = ReactiveCommand.Create(appTrayService.ShowWindow);
        Close = ReactiveCommand.Create(appTrayService.CloseApp);

        #region Commands logging
        
        Show.Subscribe(_ => this.Log().StructLogInfo($"Processing", nameof(Show)));

        Show.ThrownExceptions.Subscribe(e =>
            this.Log().StructLogError($"Processing", e.Message, nameof(Show)));

        Close.Subscribe(_ => this.Log().StructLogInfo($"Processing", nameof(Close)));

        Close.ThrownExceptions.Subscribe(e =>
            this.Log().StructLogError($"Processing", e.Message, nameof(Close)));
        
        #endregion
        
        appTrayService.ShowWindow();
    }
    
    public ReactiveCommand<Unit, Unit> Show { get; set; }
    
    public ReactiveCommand<Unit, Unit> Close { get; set; }
}