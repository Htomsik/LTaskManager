using System;
using System.Diagnostics;
using Splat;

namespace Client.Infrastructure.Logging;

/// <summary>
///     Глобальная отловка ошибок в логгер
/// <remarks> Нужен на случаи необработанных ошибок чтобы приложение не вылетало по пустякам</remarks>
/// </summary>
internal sealed class GlobalExceptionHandler : IObserver<Exception>, IEnableLogger 
{
    
    public void OnCompleted()
    {
        if(Debugger.IsAttached)
            Debugger.Break();
    }

    public void OnError(Exception error)
    {
        if(Debugger.IsAttached)
            Debugger.Break();
        
        this.Log().Fatal(error, "{0}:{1}", error.Source, error.Message);
    }

    public void OnNext(Exception error)
    {
        if(Debugger.IsAttached)
            Debugger.Break();
        
        this.Log().Fatal(error, "{0}:{1}", error.Source, error.Message);
    }
}