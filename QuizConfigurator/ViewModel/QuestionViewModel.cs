
using QuizConfigurator.Model;
using System.Collections.ObjectModel;

namespace QuizConfigurator.ViewModel;
public class QuestionViewModel : BaseViewModel
{
    private readonly Question _model;
    public string Query
    {
        get => _model.Query;
        set
        {
            _model.Query = value;
            OnPropertyChanged();
        }
    }
    public string CorrectAnswer
    {
        get => _model.CorrectAnswer;
        set
        {
            _model.CorrectAnswer = value;
            OnPropertyChanged();
        }
    }
    public ObservableCollection<string> IncorrectAnswers { get; }
    public QuestionViewModel()
    {
        
    }
    public QuestionViewModel(Question model)
    {
        _model = model;
        IncorrectAnswers = new ObservableCollection<string>(_model.IncorrectAnswers);
    }
}
