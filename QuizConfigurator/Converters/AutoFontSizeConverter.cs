using System.Globalization;
using System.Windows.Data;

namespace QuizConfigurator.Converters;
public class AutoFontSizeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string text)
        {
            if (text.Length > 30)
            {
                return 24;
            }
            return 34;
        }
        return 34;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
