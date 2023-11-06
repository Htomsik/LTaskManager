using System;
using ReactiveUI;

namespace Client.Models;

public class MenuParamCommandItem
{
    private readonly Lazy<IReactiveCommand> _lazyCommand;
    public IReactiveCommand Command => _lazyCommand.Value;
    
    public object CommandParam => _lazyCommandParam.Value;
    private readonly Lazy<object> _lazyCommandParam;
    
    public string Name { get; init; }

    
    public MenuParamCommandItem(string name, IReactiveCommand command, object commandParam)
    {
        Name = name;
        _lazyCommand = new Lazy<IReactiveCommand>(() => command);
        _lazyCommandParam = new Lazy<object>(()=> commandParam);
    }
}