﻿using System;
using System.Diagnostics;
using System.Reactive;
using Client.Infrastructure.Logging;
using Client.Services.AppInfoService.Base;
using ReactiveUI;
using Splat;


namespace Client.ViewModels;

/// <summary>
///     VMD информации о приложении
/// </summary>
internal sealed class AppInfoViewModel : ViewModelBase
{
    /// <summary>
    ///     Сервис информации о приложении
    /// </summary>
    public IAppInfoService AppInfoService { get; }
    
    /// <param name="appInfoService"> Сервис предоставляющий информацию о приложении</param>
    public AppInfoViewModel(IAppInfoService appInfoService)
    {
        AppInfoService = appInfoService;
        
        OpenGitHubUrl = ReactiveCommand.Create(() =>
        {
            Process.Start(new ProcessStartInfo{FileName = AppInfoService.AppGitHub, UseShellExecute = true});
        });

        #region Command logging

        OpenGitHubUrl.ThrownExceptions.Subscribe(e =>
            this.Log().StructLogError($"Processing", e.Message, nameof(OpenGitHubUrl)));
        
        OpenGitHubUrl.Subscribe(_ => this.Log().StructLogInfo($"Processing", nameof(OpenGitHubUrl)));

        #endregion
    }
    public ReactiveCommand<Unit, Unit> OpenGitHubUrl { get; set; }
}