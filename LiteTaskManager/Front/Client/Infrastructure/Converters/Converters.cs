using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Client.Extensions;

namespace Client.Infrastructure.Converters;

/// <summary>
///     Конвертер обьекта в его описание
/// </summary>
public class ObjectToStringConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is null ? string.Empty : value.ToString();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}


/// <summary>
///     Конвертер процентов в цвета
/// </summary>
public class DoublePercentToColor : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        IBrush? brush = null;
        
        if (value is not double percent)
        {
            return brush;
        }
        
        brush = percent switch
        {
            >= 75 => ResourcesExtension.Percent75Brush,
            >= 50 => ResourcesExtension.Percent50Brush,
            >= 25 => ResourcesExtension.Percent25Brush,
            > 0 => ResourcesExtension.Percent0Brush,
            _ => brush
        };

        return brush;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}