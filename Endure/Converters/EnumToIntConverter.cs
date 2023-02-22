using System.Globalization;

namespace Endure.Converters;

public class EnumToIntConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (int)value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return Enum.ToObject(targetType, (int)value);
    }
}