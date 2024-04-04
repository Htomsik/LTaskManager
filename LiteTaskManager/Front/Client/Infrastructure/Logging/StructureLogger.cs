using System;
using System.Diagnostics;
using System.Reactive.Joins;
using System.Runtime.CompilerServices;
using Splat;

namespace Client.Infrastructure.Logging;

/// <summary>
///      Расширение для логгера
/// </summary>
public static class StructureLogger
{
    #region Const

    private const string Pattern  = "[{caller}:{callerMethod}]: {text}";

    private const string ErrorPattern = "Have error:";

    #endregion

    #region Logging
    
    public static void StructLogInfo(this IFullLogger logger, 
        string text,
        [CallerMemberName] string callerMethod = "",
        [CallerFilePath] string callerClass = "")
    {
        logger.StructLog(LogLevel.Info, text, callerMethod, callerClass);
    }
    
    public static void StructLogDebug(this IFullLogger logger, 
        string text,
        string errorMessage = "",
        [CallerMemberName] string callerMethod = "",
        [CallerFilePath] string callerClass = "")
    {
        if (!string.IsNullOrEmpty(errorMessage))
        {
            errorMessage = string.Concat(ErrorPattern, errorMessage);
        }
        logger.StructLog(LogLevel.Debug, $"{text}.{errorMessage}", callerMethod, callerClass);
    }
    
    public static void StructLogWarn(this IFullLogger logger, 
        string text,
        [CallerMemberName] string callerMethod = "",
        [CallerFilePath] string callerClass = "")
    {
        logger.StructLog(LogLevel.Warn, text, callerMethod, callerClass);
    }
    
    
    public static void StructLogError(this IFullLogger logger, 
        string text,
        string errorMessage,
        [CallerMemberName] string callerMethod = "",
        [CallerFilePath] string callerClass = "")
    {
        if (!string.IsNullOrEmpty(errorMessage))
        {
            errorMessage = string.Concat(ErrorPattern, errorMessage);
        }
        logger.StructLog(LogLevel.Error, $"{text}.{errorMessage}", callerMethod, callerClass);
    }
    
    public static void StructLogFatal(this IFullLogger logger, 
        string text,
        [CallerMemberName] string callerMethod = "",
        [CallerFilePath] string callerClass = "")
    {
        logger.StructLog(LogLevel.Fatal, text, callerMethod, callerClass);
    }
    
    
    public static void StructLog(this IFullLogger logger, 
        LogLevel logLevel,
        string text,
        [CallerMemberName] string callerMethod = "",
        [CallerFilePath] string callerClass = "")
    {
        // TODO: Если будут просадки придумать способ как оптимизировать
        var path = callerClass.Split("\\");

        callerClass = path[^1];

        switch (logLevel)
        {
            case LogLevel.Error:
                logger.Error(Pattern, callerClass, callerMethod, text);
                break;
            
            case LogLevel.Fatal:
                logger.Fatal(Pattern, callerClass, callerMethod, text);
                break;
            
            case LogLevel.Warn:
                logger.Warn(Pattern, callerClass, callerMethod, text);
                break;
            
            case LogLevel.Debug:
                logger.Debug(Pattern, callerClass, callerMethod, text);
                break;

            case LogLevel.Info:
            default:
                logger.Info(Pattern, callerClass, callerMethod, text);
                break;
        }
        
    }

    #endregion

    #region TimeLogging

    /// <summary>
    ///     Логирует время работы блока кода, не поддерживает возврат
    /// </summary>
    public static void TimeLog(this  Action action,
        IFullLogger logger,
        [CallerMemberName] string callerMethod = "",
        [CallerFilePath] string callerClass = "") {
        
        logger.StructLogWarn("Start processing", callerMethod, callerClass);
        
        var sw = new Stopwatch();
        sw.Start();

        try
        {
            action();
        }
        catch (Exception e)
        {
            sw.Stop();
            
            logger.StructLogError( $"End processing. Elapsed time: {sw.ElapsedMilliseconds} ms", e.Message,  callerMethod, callerClass);
            
            return;
        }
        
        sw.Stop();
        
        logger.StructLogWarn( $"End processing. Elapsed time: {sw.ElapsedMilliseconds} ms", callerMethod, callerClass);
    }

    #endregion
}