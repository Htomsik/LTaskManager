using System;
using System.Collections.Generic;
using System.Globalization;
using Client.Infrastructure.Logging;
using Client.Models;
using Splat;

namespace Client.Services.AppCultureService;

internal sealed class AppCultureService : IAppCultureService, IEnableLogger
{
    /// <summary>
    ///     Словарь сопоставления enum в string
    /// <remarks>   Для задания локализации требуется строка    </remarks>
    /// </summary>
    private readonly Dictionary<AppCulture, string> _culturesStr = new()
    {
        { AppCulture.En , "EN-en"},
        { AppCulture.Rus, "Ru-ru"}
    };
    
    public bool SetCulture(AppCulture appCulture)
    {
        if (!_culturesStr.TryGetValue(appCulture, out var value))
        {
            this.Log().StructLogFatal($"Localization for culture {appCulture} doesn't exist");
            return false;
        }
        
        try
        {
            Assets.Resources.Culture = new CultureInfo(value);
        }
        catch (Exception e)
        {
            this.Log().StructLogError($"Localization for culture {appCulture} doesn't exist", e.Message);
            return false;
        }

        return true;
    }
} 