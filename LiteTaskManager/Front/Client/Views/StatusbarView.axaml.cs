using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Client.Views;

public partial class StatusbarView : UserControl
{
    public StatusbarView()
    {
        InitializeComponent();
    }
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}