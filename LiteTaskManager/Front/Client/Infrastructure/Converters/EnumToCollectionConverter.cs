using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Client.Infrastructure.Converters;

/// <summary>
///     Конвертирует enum тип в коллекцию содержащую все возможные его значения
/// </summary>
public class EnumToCollectionConverter : IValueConverter
{
    /// <summary>
    ///     Хранилище уже созданных коллекций по типам 
    /// </summary>
    private static readonly Dictionary<Type, IList> EnumCollections = new();
    
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null)
            return null;

        var enumType = value.GetType();
        
        if (!enumType.IsEnum)
        {
            return null;
        }

        IList? list;

        if (EnumCollections.TryGetValue(enumType, out var collection))
        {
            list = collection;
        }
        else
        {
            var listType = typeof(List<>).MakeGenericType(enumType);
            list = (IList)Activator.CreateInstance(listType)!;
            foreach (var elem in Enum.GetValues(enumType))
            {
                list.Add(elem);
            }
            
            EnumCollections.Add(enumType, list);
        }
        return list;
    }

    
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }
}