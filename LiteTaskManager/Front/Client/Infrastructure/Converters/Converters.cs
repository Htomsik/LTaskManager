using System;
using System.Globalization;
using Avalonia.Data.Converters;

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