using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Client.Views;

public partial class AppSettingsView : UserControl
{
    public AppSettingsView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}