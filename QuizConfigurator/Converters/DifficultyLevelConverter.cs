using QuizConfigurator.Model;
using System.Globalization;
using System.Windows.Data;

namespace QuizConfigurator.Converters;
public class DifficultyLevelConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Difficulty difficulty)
        {
            return (int)difficulty; 
        }
        return 1;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int intValue)
        {
            return (Difficulty)intValue;
        }
        return Difficulty.Medium; 
    }
}
