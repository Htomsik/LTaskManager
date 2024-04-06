using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Client.ViewModels;
using Client.Views;
using ReactiveUI;
using Splat;

namespace Client.Infrastructure;

/// <summary>
///      Выборка нужной view по типу viewModel
/// <remarks> С помошью него Avalonia понимает какую вьюшку нужно вывести в ContentControl к которой прибинджена viewmodel</remarks>
/// </summary>
internal sealed class ViewLocator : IDataTemplate, IEnableLogger
{
    private readonly Dictionary<Type, Type> _vmdToViewTypes = new()
    {
        {typeof(ProcessesViewModel),typeof(ProcessesView)},
        {typeof(AppSettingsViewModel), typeof(AppSettingsView)},
        {typeof(StatusBarViewModel), typeof(StatusbarView)},
        {typeof(AppInfoViewModel), typeof(AppInfoView)},
        {typeof(AgreementViewModel), typeof(AgreementView)}
    };
    
    
    public Control Build(object? vmd)
    {
        var view = new Control();
        
        try
        {
            view = (Control)Activator.CreateInstance(typeof(NoDataView))!;

            if (vmd is not null)
            {
                var viewType = _vmdToViewTypes[vmd.GetType()];
            
                view = (Control)Activator.CreateInstance(viewType)!;
            }
        }
        catch(Exception e)
        {
            this.Log().Error($"Could not find the view for view model {vmd?.GetType().FullName}. {e.Message}");
        }
        
        return view;
    }

    public bool Match(object? data) => data is ReactiveObject;
}