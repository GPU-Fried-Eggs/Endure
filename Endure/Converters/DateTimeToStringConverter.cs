using System.Globalization;

namespace Endure.Converters;

public class DateTimeToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return ((DateTime)value).ToString("hh:mm:ss");
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var data = DateTime.TryParse(value.ToString(), out var result);
        return data ? result : new DateTime();
    }
}
