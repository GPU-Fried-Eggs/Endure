using System.Globalization;

namespace Endure.Converters;

public class EnumToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value.ToString() ?? string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is string str
            ? Enum.Parse(targetType, str, true)
            : Enum.ToObject(targetType, 0);
    }
}