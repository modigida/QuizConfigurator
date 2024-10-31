using System.Globalization;
using System.Windows.Data;

namespace QuizConfigurator.Converters;
public class AutoFontSizeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string text)
        {
            // Här kan du justera min/max storlek och logik för att beräkna storlek
            if (text.Length > 30) // Justera detta värde beroende på vad som anses "för lång"
            {
                return 24; // Minska storleken
            }
            return 34; // Standard storlek
        }
        return 34; // Standard storlek om inget värde
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
