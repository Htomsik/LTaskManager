using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Client.Models;
using Material.Icons;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace Client.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public IEnumerable<MenuParamCommandItem> MenuList { get; set; }

    [Reactive] 
    public ViewModelBase CurrentViewModel { get; set; }
    
    
    public MainWindowViewModel()
    {
        CurrentViewModel = Locator.Current.GetService<ProcessesViewModel>();
        
        Navigate = ReactiveCommand.Create<Type>(
            type =>
            {
                CurrentViewModel = (Locator.Current.GetService(type) as ViewModelBase)!;
            });

        Navigate.ThrownExceptions.Subscribe(x => this.Log().Error($"Execptions then processing {nameof(Navigate)} command:{x.Message}"));
        
        MenuList = new ObservableCollection<MenuParamCommandItem>
        { 
            new MenuParamCommandItem("Processes", Navigate, typeof(ProcessesViewModel), MaterialIconKind.Memory)
        };
    }
    
    public IReactiveCommand Navigate { get; init; }
}