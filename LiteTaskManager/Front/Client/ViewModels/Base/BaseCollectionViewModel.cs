using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;


namespace Client.ViewModels;

/// <summary>
///     Вьюмодель для отображения изменяющейся в реалтайме коллекции
/// </summary>
internal abstract class BaseCollectionViewModel<T> : ViewModelBase where T : notnull
{
    #region Prooperties
    
    /// <summary>
    ///     Коллекция для отображения во вьюшке
    /// </summary>
    public ReadOnlyObservableCollection<T>? Items => SelectedItems;
    
    /// <summary>
    ///     Тест поиска
    /// </summary>
    [Reactive]
    public string? SearchText { get; set; }
    
    #endregion

    #region Fields
    
    /// <summary>
    ///     Коллекция с которой взаимодействуем из кода
    /// </summary>
    protected ReadOnlyObservableCollection<T>? SelectedItems;
    
    /// <summary>
    ///     Подписка на обновление коллекции
    /// </summary>
    protected IDisposable? ItemsSubscriptions;
    
    /// <summary>
    ///     Наблюдатель за свойством поиска
    /// </summary>
    protected IObservable<Func<T, bool>> SearchFilter = null!;
    
    #endregion

    #region Constructors

    public BaseCollectionViewModel()
    {
        ClearSearchText = ReactiveCommand.Create(() => { SearchText = string.Empty;});

        ClearSearchText.ThrownExceptions.Subscribe(e => this.Log().Error($"Can't clear {nameof(SearchText)}. {e.Message}"));
        ClearSearchText.Subscribe(_ => this.Log().Info($"{nameof(ClearSearchText)} executed"));
    }

    #endregion

    #region Commands

    /// <summary>
    ///     Очистка строки поиска
    /// </summary>
    public ReactiveCommand<Unit, Unit> ClearSearchText { get; } 

    #endregion
    
    #region Methods
    
    /// <summary>
    ///     Установка подисок на фильтры
    /// </summary>
    protected virtual void SetFiltersSubscriptions()
    {
        SearchFilter = null!;
        
        SearchFilter =
            this.WhenValueChanged(x => x.SearchText)
                .Throttle(TimeSpan.FromMicroseconds(250))
                .Select(SearchFilterBuilder);
    }

    /// <summary>
    ///     Установка подписок на обновление для коллекции
    /// </summary>
    protected virtual void SetItemsSubscriptions(ObservableCollection<T> inputData)
    {
        // Предварительная утилизация старой подписки
        ItemsSubscriptions?.Dispose();

        ItemsSubscriptions =
            inputData
                .ToObservableChangeSet()
                .Filter(SearchFilter)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out SelectedItems)
                .DisposeMany()
                .Subscribe();
    }

    /// <summary>
    ///     Фильтр поиска
    /// </summary>
    /// <param name="text">Введенный текст</param>
    /// <returns> Возвращает функцию поиска в зависимости от параметров</returns>
    protected virtual Func<T, bool> SearchFilterBuilder(string? text)
    {
        text = text?.Trim().ToLower();

        if (string.IsNullOrEmpty(text)) return _ => true;
        
        return entity => entity.ToString()!.Contains(text, StringComparison.OrdinalIgnoreCase);
    }

    #endregion
}