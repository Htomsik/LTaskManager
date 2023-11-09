using System;
using System.Windows.Input;
using Material.Icons;
using ReactiveUI;

namespace Client.Models;

internal sealed class MenuParamCommandItem
{
    private readonly Lazy<ICommand> _lazyCommand;
    public ICommand Command => _lazyCommand.Value;
    
    public object CommandParam => _lazyCommandParam.Value;
    private readonly Lazy<object> _lazyCommandParam;
    
    public string Name { get; init; }

    public MaterialIconKind Kind { get; init; }
    
    public bool UseKind { get; init; } 
    
    public MenuParamCommandItem(string name, IReactiveCommand command, object commandParam, MaterialIconKind kind)
    {
        Name = name;
        Kind = kind;
        UseKind = true;
        _lazyCommand = new Lazy<ICommand>(() => (ICommand)command);
        _lazyCommandParam = new Lazy<object>(()=> commandParam);
    }
    
    public MenuParamCommandItem(string name, IReactiveCommand command, object commandParam)
    {
        Name = name;
        UseKind = false;
        _lazyCommand = new Lazy<ICommand>(() => (ICommand)command);
        _lazyCommandParam = new Lazy<object>(()=> commandParam);
    }
}