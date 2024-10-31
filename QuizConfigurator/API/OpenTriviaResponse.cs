using System.Collections.ObjectModel;

namespace QuizConfigurator.API;
public class OpenTriviaResponse
{
    public int? response_code { get; set; }
    public OpenTriviaQuestion[] results { get; set; }
    public ObservableCollection<OpenTriviaCategories> trivia_categories { get; set; }

    public static readonly Dictionary<int, string> ResponseMessages = new Dictionary<int, string>
    {
        { 0, "Questions loaded successfully." },
        { 1, "No results for the selected parameters." },
        { 2, "Invalid parameter values." },
        { 3, "Token not found or expired." },
        { 4, "Token empty; request reset." }
    };
    public OpenTriviaResponse()
    {
        trivia_categories = new ObservableCollection<OpenTriviaCategories>();
    }
    public static string GetResponseMessage(int? responseCode)
    {
        return responseCode.HasValue && ResponseMessages.ContainsKey(responseCode.Value)
            ? ResponseMessages[responseCode.Value]
            : "Unknown response code.";
    }
}
