﻿using System;
using System.IO;
using System.Threading.Tasks;
using AppInfrastructure.Services.FileService;
using AppInfrastructure.Services.ParserService;
using AppInfrastructure.Stores.DefaultStore;
using Avalonia.Controls;
using Client.Infrastructure.Logging;
using Client.Models;
using Client.Services.AppCultureService;
using Client.Services.AppTrayService;
using Splat;

namespace Client.Services.FileServices;

/// <summary>
///     Отвечает за ввод/вывод настроек приложения из файла конфигурции
/// </summary>
internal sealed class AppSettingsStoreFileService : BaseStoreFileService<IStore<AppSettings>, AppSettings>, IEnableLogger
{
    private readonly IAppCultureService _appCultureService;
    
    private readonly IAppTrayService _appTrayService;
    
    public AppSettingsStoreFileService(IStore<AppSettings> store, IParserService parserService, IAppCultureService appCultureService, IAppTrayService appTrayService) : base(store, parserService, $"{nameof(AppSettings)}.js", Path.Combine(Directory.GetCurrentDirectory(), "Data"))
    {
        _appCultureService = appCultureService;
        _appTrayService = appTrayService;
    }
    
    public override async Task<bool> GetAsync()
    {
        var result = false;
        
        try
        {
             result = await base.GetAsync(); 
        }
        catch (Exception e)
        {
            this.Log().StructLogError("Can't get file settings", e.Message);
        }
        
        if (result)
        {
            AfterGet();
            return result;
        }
        
        this.Log().StructLogWarn($"Recreating {FileName}");
        
        result = await SetAsync(); 
        
        if (result)
        { 
            result = await base.GetAsync(); 
        }

        if (result)
        {
            AfterGet();
        }
        
        return result;
    }

    private void AfterGet()
    {
        _appCultureService.SetCulture(Store.CurrentValue.Culture);
        _appTrayService.ChangeShutdownPolitic(Store.CurrentValue.ShutdownToTray ? ShutdownMode.OnExplicitShutdown : ShutdownMode.OnMainWindowClose);
    }

    public override async Task<bool> SetAsync()
    {
        var result = await base.SetAsync(); 
        
        return result;
    }
}