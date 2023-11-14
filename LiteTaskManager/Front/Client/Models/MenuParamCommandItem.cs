using System;
using System.Windows.Input;
using Material.Icons;
using ReactiveUI;

namespace Client.Models;

/// <summary>
///     Модель для создания коллекций команд
/// <remarks> Например, можно сделать навигационную менюшку</remarks>
/// </summary>
internal sealed class MenuParamCommandItem
{
    #region Fields

    private readonly Lazy<ICommand> _lazyCommand;
    
    private readonly Lazy<object> _lazyCommandParam;

    #endregion

    #region Properties

    /// <summary>
    ///     Выполняемая данным действием команда
    /// </summary>
    public ICommand Command => _lazyCommand.Value;
    
    /// <summary>
    ///     Параметр для команды
    /// </summary>
    public object CommandParam => _lazyCommandParam.Value;
    
    /// <summary>
    ///     Наименование действия/Объекта
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    ///     Иконка
    /// </summary>
    public MaterialIconKind Kind { get; init; }
    
    /// <summary>
    ///     Задана ли иконка
    /// </summary>
    public bool UseKind { get; init; } 

    #endregion
    
    /// <param name="name"> Наименование действия/Объекта </param>
    /// <param name="command"> Команда, связанная с данным действием </param>
    /// <param name="commandParam"> Параметр команды, передаваемый из вне</param>
    /// <param name="kind"> Отображаемая иконк </param>
    public MenuParamCommandItem(string name, IReactiveCommand command, object commandParam, MaterialIconKind kind)
    {
        Name = name;
        Kind = kind;
        UseKind = true;
        _lazyCommand = new Lazy<ICommand>(() => (ICommand)command);
        _lazyCommandParam = new Lazy<object>(()=> commandParam);
    }
    
    /// <param name="name"> Наименование действия/Объекта </param>
    /// <param name="command"> Команда, связанная с данным действием </param>
    /// <param name="commandParam"> Параметр команды, передаваемый из вне</param>
    public MenuParamCommandItem(string name, IReactiveCommand command, object commandParam)
    {
        Name = name;
        UseKind = false;
        _lazyCommand = new Lazy<ICommand>(() => (ICommand)command);
        _lazyCommandParam = new Lazy<object>(()=> commandParam);
    }
}