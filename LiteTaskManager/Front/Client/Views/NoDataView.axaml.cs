﻿using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Client.Views;

public partial class NoDataView : UserControl
{
    public NoDataView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}