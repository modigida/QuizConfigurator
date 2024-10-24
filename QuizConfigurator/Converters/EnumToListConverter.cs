using QuizConfigurator.Model;
using System.Globalization;
using System.Windows.Data;

namespace QuizConfigurator.Converters;

public class EnumToListConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return Enum.GetValues(typeof(Difficulty)).Cast<Difficulty>().ToList();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (Difficulty)value;
    }
}
