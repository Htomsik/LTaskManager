using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Client.Views;

public partial class AgreementView : UserControl
{
    public AgreementView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}