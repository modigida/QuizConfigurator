
using System.Windows.Threading;

namespace QuizConfigurator.ViewModel;
public class PlayerViewModelTimer : BaseViewModel
{
    private MainWindowViewModel _mainWindowViewModel;
    private PlayerViewModel _playerViewModel;

    public DispatcherTimer Timer;

    public PlayerViewModelTimer(MainWindowViewModel mainWindowViewModel)
    {
        _mainWindowViewModel = mainWindowViewModel;
    }
    internal void InitializeTimer(PlayerViewModel playerViewModel)
    {
        _playerViewModel = playerViewModel;

        Timer = new DispatcherTimer();
        Timer.Interval = TimeSpan.FromSeconds(1);
        Timer.Tick += Timer_Tick;
    }
    public void StartTimer()
    {
        if (_mainWindowViewModel.ActivePack != null)
        {
            _playerViewModel.RemainingTime = _mainWindowViewModel.ActivePack.TimeLimitInSeconds;
            Timer.Start();
        }
    }
    public void Timer_Tick(object? sender, EventArgs e)
    {
        if (_playerViewModel.RemainingTime > 0)
        {
            if (_playerViewModel.RemainingTime <= 6 && _playerViewModel.IsSoundOn)
            {
                PlayTickSound();
            }

            _playerViewModel.RemainingTime--;
        }
        else
        {
            Timer.Stop();
            _playerViewModel.LoadNextQuestion();
        }
    }
    private void PlayTickSound()
    {
        try
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer("Resources\\tick.wav");
            player.Play();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
