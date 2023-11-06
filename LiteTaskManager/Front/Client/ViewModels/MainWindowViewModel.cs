using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Client.Models;
using ReactiveUI;
using Splat;

namespace Client.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public IEnumerable<MenuParamCommandItem> MenuList { get; set; }

    public MainWindowViewModel()
    {
        MenuList = new ObservableCollection<MenuParamCommandItem>
        {
            new MenuParamCommandItem("test", ReactiveCommand.Create(()=>Console.WriteLine("124")), null)
        };
    }
}