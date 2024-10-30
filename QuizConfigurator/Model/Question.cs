

using System.Text.Json.Serialization;

namespace QuizConfigurator.Model;
public class Question
{
    public string? Query { get; set; }
    public string? CorrectAnswer { get; set; }
    public string[]? IncorrectAnswers { get; set; }
    public Question(string query, string correctAnswer, string incorrectAnswerOne,
        string incorrectAnswerTwo, string incorrectAnswerThree)
    {
        Query = query;
        CorrectAnswer = correctAnswer;
        IncorrectAnswers = new string[3] { incorrectAnswerOne, incorrectAnswerTwo, incorrectAnswerThree };
    }

    [JsonConstructor]
    public Question(string? query, string? correctAnswer, string[]? incorrectAnswers)
    {
        Query = query;
        CorrectAnswer = correctAnswer;
        IncorrectAnswers = incorrectAnswers ?? new string[3];
    }
}
