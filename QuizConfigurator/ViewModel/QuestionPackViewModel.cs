using QuizConfigurator.Model;
using System.Collections.ObjectModel;

namespace QuizConfigurator.ViewModel;
public class QuestionPackViewModel : BaseViewModel
{
    private readonly QuestionPack _model;
    public string Name
    {
        get => _model.Name;
        set
        {
            _model.Name = value;
            OnPropertyChanged();
        }
    }
    public Difficulty Difficulty
    {
        get => _model.Difficulty;
        set
        {
            _model.Difficulty = value;
            OnPropertyChanged();
        }
    }
    public int TimeLimitInSeconds
    {
        get => _model.TimeLimitInSeconds;
        set
        {
            _model.TimeLimitInSeconds = value;
            OnPropertyChanged();
        }
    }
    public ObservableCollection<QuestionViewModel> Questions { get; }
    public QuestionPackViewModel()
    {
        
    }
    public QuestionPackViewModel(QuestionPack model)
    {
        _model = model;

        Questions = new ObservableCollection<QuestionViewModel>(
            _model.Questions.Select(q => new QuestionViewModel(q))
        );
    }
}
