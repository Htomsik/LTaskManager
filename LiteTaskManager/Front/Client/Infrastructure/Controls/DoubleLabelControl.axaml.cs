using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Client.Infrastructure.Controls;

public class DoubleLabelControl : TemplatedControl
{
    #region Header

    public static readonly StyledProperty<string> HeaderProperty =
        AvaloniaProperty.Register<DoubleLabelControl, string>(nameof(Header), defaultValue: "Header");

    public string Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, $"{value}{HeaderPostfixProperty}");
    }

    #endregion

    #region HeaderPostFix

    public static readonly StyledProperty<string> HeaderPostfixProperty = AvaloniaProperty.Register<DoubleLabelControl, string>(
        ":");

    public string HeaderPostfix
    {
        get => GetValue(HeaderPostfixProperty);
        set => SetValue(HeaderPostfixProperty, value);
    }

    #endregion


    #region Body

    public static readonly StyledProperty<string> BodyProperty =
        AvaloniaProperty.Register<DoubleLabelControl, string>(nameof(Body), defaultValue: "Body");

    public string Body
    {
        get => GetValue(BodyProperty);
        set => SetValue(BodyProperty, value);
    }

    #endregion
    
}