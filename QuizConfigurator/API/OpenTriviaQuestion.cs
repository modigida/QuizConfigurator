
namespace QuizConfigurator.API;
public class OpenTriviaQuestion
{
    public string? category { get; set; }
    public string? difficulty { get; set; }
    public string? question { get; set; }
    public string? correct_answer { get; set; }
    public string[]? incorrect_answers { get; set; }
}
