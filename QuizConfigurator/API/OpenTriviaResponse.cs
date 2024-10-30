
using System.Collections.ObjectModel;

namespace QuizConfigurator.API;
public class OpenTriviaResponse
{
    public int? response_code { get; set; }
    public OpenTriviaQuestion[] results { get; set; }
    public ObservableCollection<OpenTriviaCategories> trivia_categories { get; set; }
    public OpenTriviaResponse()
    {
        trivia_categories = new ObservableCollection<OpenTriviaCategories>();
    }
}
