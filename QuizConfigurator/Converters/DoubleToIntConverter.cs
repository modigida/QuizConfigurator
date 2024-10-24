using System.Globalization;
using System.Windows.Data;

namespace QuizConfigurator.Converters;
public class DoubleToIntConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if(value != null && value is double doubleValue)
        {
            return (int)doubleValue;
        }
        else if (value != null && value is int intValue)
        {
            return intValue;
        }
        throw new InvalidCastException("Expected value to be of type double.");
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return System.Convert.ToDouble(value);
    }
}
