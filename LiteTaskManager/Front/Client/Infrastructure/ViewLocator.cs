using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Client.ViewModels;
using Client.Views;
using ReactiveUI;
using Splat;

namespace Client.Infrastructure;

public class ViewLocator : IDataTemplate, IEnableLogger
{
    private readonly Dictionary<Type, Type> _vmdToViewTypes = new()
    {
        {typeof(ProcessesViewModel),typeof(ProcessesView)}
    };
    
    
    public IControl Build(object vmd)
    {
        IControl view = null!;
        
        try
        {
            view = (Control)Activator.CreateInstance(typeof(NoDataView))!;
            
            var viewType = _vmdToViewTypes[vmd.GetType()];
            
            view = (Control)Activator.CreateInstance(viewType)!;
        }
        catch(Exception error)
        {
            this.Log().Error($"Could not find the view  for view model {vmd.GetType().FullName}.");
        }
        
        return view;
    }

    public bool Match(object data) => data is ReactiveObject;
}