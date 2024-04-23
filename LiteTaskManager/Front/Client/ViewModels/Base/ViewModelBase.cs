using System;
using System.Reactive.Disposables;
using ReactiveUI;

namespace Client.ViewModels;

/// <summary>
///     Базовая вьюмодель
/// </summary>
internal abstract class ViewModelBase : ReactiveObject, IDisposable
{
    /// <summary>
    ///     Привязка для удаления зависимостей
    /// </summary>
    protected readonly CompositeDisposable CompositeDisposable = new ();
    
    public virtual void Dispose()
    {
        CompositeDisposable.Dispose();
    }
}