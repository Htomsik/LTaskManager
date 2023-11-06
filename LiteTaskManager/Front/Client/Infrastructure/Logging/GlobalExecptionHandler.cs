using System;
using System.Diagnostics;
using Splat;

namespace Client.Infrastructure.Logging;

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