
using QuizConfigurator.Model;
using System.Windows.Input;
using System.Windows.Threading;

namespace QuizConfigurator.ViewModel;
public class PlayerViewModel : BaseViewModel
{
    private readonly MainWindowViewModel? _mainWindowViewModel;
    private DispatcherTimer _timer; // dispatcher.invoke goolge
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

        _timer = new DispatcherTimer();
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += (sender, e) =>
        {
            //PlayerViewModel.TimeToAnswer--;
            //OnPropertyChanged(nameof(PlayerViewModel.TimeToAnswer)); -- maybe not needed, since in property
            //if (PlayerViewModel.TimeToAnswer == 0)
            //{
            //    PlayerViewModel.Stop();
            //}
        };
        _timer.Start();
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