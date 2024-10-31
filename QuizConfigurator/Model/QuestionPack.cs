using System.Text.Json.Serialization;

namespace QuizConfigurator.Model;
public class QuestionPack
{
    public string Name { get; set; }
    public Difficulty Difficulty { get; set; }
    public int TimeLimitInSeconds { get; set; }
    public List<Question> Questions { get; set; }
    public QuestionPack(string name, Difficulty difficulty = Difficulty.Medium, int timeLimitInSeconds = 30)
    {
        Name = name;
        Difficulty = difficulty;
        TimeLimitInSeconds = timeLimitInSeconds;
        Questions = new List<Question>();
    }

    [JsonConstructor]
    public QuestionPack(string name, Difficulty difficulty, int timeLimitInSeconds, List<Question> questions)
    {
        Name = name;
        Difficulty = difficulty;
        TimeLimitInSeconds = timeLimitInSeconds;
        Questions = questions ?? new List<Question>();
    }
}
