
using QuizConfigurator.Model;
using System.Windows.Input;

namespace QuizConfigurator.ViewModel;
public class PlayerViewModel : BaseViewModel
{
    private readonly MainWindowViewModel? _mainWindowViewModel;
    private List<Question>? _questions;
    private List<string>? _answers;
    private int correctAnswerIndex;
    private int _timeToAnswer;
    private bool _isPlaying;
    public bool IsPlaying
    {
        get => _isPlaying;
        set
        {
            _isPlaying = value;
            OnPropertyChanged();
        }
    }
    public ICommand PickAnswerCommand { get; }
    public ICommand RestartGameCommand { get; }
    public PlayerViewModel(MainWindowViewModel? mainWindowViewModel)
    {
        _mainWindowViewModel = mainWindowViewModel;
    }
    public void Start()
    {
        IsPlaying = true;
    }
    public void Stop()
    {
        IsPlaying = false;
        _mainWindowViewModel.IsPlayMode = false;
    }
}