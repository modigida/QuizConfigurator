using QuizConfigurator.Commands;
using System.Speech.Synthesis;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace QuizConfigurator.ViewModel;
public class PlayerViewModel : BaseViewModel
{
    private SpeechSynthesizer? _speechSynthesizer;
    public readonly MainWindowViewModel MainWindowViewModel;

    private DispatcherTimer _timer;

    private List<QuestionViewModel>? _randomizedQuestions;
    private QuestionViewModel _currentQuestion;
    public QuestionViewModel CurrentQuestion
    {
        get => _currentQuestion;
        set
        {
            _currentQuestion = value;
            OnPropertyChanged();
        }
    }

    private List<string> _currentAnswerOptions;
    public List<string> CurrentAnswerOptions
    {
        get => _currentAnswerOptions;
        set
        {
            _currentAnswerOptions = value;
            OnPropertyChanged();
        }
    }

    private int _correctAnswerIndex;
    public int CurrentQuestionNumber { get; set; }
    public int AmountOfCorrectAnswers { get; set; }
    public int AmountOfQuestions
    {
        get => _randomizedQuestions?.Count ?? 0;
    }

    private int _remainingTime;
    public int RemainingTime
    {
        get => _remainingTime;
        private set
        {
            _remainingTime = value;
            OnPropertyChanged();
        }
    }
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
    private bool _isSoundOn;
    public bool IsSoundOn
    {
        get => _isSoundOn;
        private set
        {
            _isSoundOn = value;
            OnPropertyChanged();
        }
    }
    public ICommand PickAnswerCommand { get; }
    public ICommand RestartGameCommand { get; }
    public ICommand SetSoundSettingCommand { get; }
    public PlayerViewModel(MainWindowViewModel mainWindowViewModel)
    {
        CreateVoiceFeature();

        MainWindowViewModel = mainWindowViewModel;

        _timer = new DispatcherTimer();
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += Timer_Tick;

        PickAnswerCommand = new RelayCommand(PickAnswer);
        RestartGameCommand = new RelayCommand(RestartGame);
        SetSoundSettingCommand = new RelayCommand(ToggleSound);

        _isSoundOn = true;
    }
    private void CreateVoiceFeature()
    {
        _speechSynthesizer = new SpeechSynthesizer();
        _speechSynthesizer.SetOutputToDefaultAudioDevice();
        _speechSynthesizer.SelectVoice("Microsoft Zira Desktop");
    }
    private void ToggleSound(object obj)
    {
        _isSoundOn = !_isSoundOn;
        OnPropertyChanged(nameof(IsSoundOn));
    }
    public async Task ExecuteVoice(string textToRead)
    {
        if (_isSoundOn && CurrentQuestion != null && _speechSynthesizer != null)
        {
            await Task.Run(() => _speechSynthesizer.Speak(textToRead));
        }
    }
    public void Start()
    {
        var result = MessageBox.Show("Play with sound?", "Sound Setting", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result == MessageBoxResult.Yes)
        {
            _isSoundOn = true;
            OnPropertyChanged(nameof(IsSoundOn));
        }
        else
        {
            _isSoundOn = false;
            OnPropertyChanged(nameof(IsSoundOn));
        }
        if (MainWindowViewModel.ActivePack != null)
        {
            _randomizedQuestions = MainWindowViewModel.ActivePack.Questions.ToList();
            InitializeQuestions(_randomizedQuestions);
            IsPlaying = true;
        }
    }
    private void InitializeQuestions(List<QuestionViewModel> questions)
    {
        var random = new Random();
        _randomizedQuestions = questions.OrderBy(q => random.Next()).ToList();
        CurrentQuestionNumber = 0;
        LoadNextQuestion();
    }
    private async Task LoadNextQuestion()
    {
        if (_randomizedQuestions != null && CurrentQuestionNumber < _randomizedQuestions.Count)
        {
            CurrentQuestion = _randomizedQuestions[CurrentQuestionNumber];
            CurrentQuestionNumber++;

            var allAnswers = new List<string>(CurrentQuestion.IncorrectAnswers)
            {
                CurrentQuestion.CorrectAnswer
            };

            var random = new Random();
            CurrentAnswerOptions = allAnswers.OrderBy(a => random.Next()).ToList();

            _correctAnswerIndex = CurrentAnswerOptions.IndexOf(CurrentQuestion.CorrectAnswer);

            StartTimer();

            if (IsSoundOn)
            {
                await Task.Delay(500);
                string stringToRead = CurrentQuestion.Query;
                await ExecuteVoice(stringToRead);
            }
        }
        else
        {
            Stop();
        }
    }
    public void StartTimer()
    {
        if (MainWindowViewModel.ActivePack != null)
        {
            RemainingTime = MainWindowViewModel.ActivePack.TimeLimitInSeconds;
            _timer.Start();
        }
    }
    private void Timer_Tick(object sender, EventArgs e)
    {
        if (RemainingTime > 0)
        {
            if (RemainingTime <= 6 && IsSoundOn)
            {
                PlayTickSound();
            }

            RemainingTime--;
        }
        else
        {
            _timer.Stop();
            LoadNextQuestion();
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
            Console.WriteLine("Kunde inte spela tick-ljudet: " + ex.Message);
        }
    }
    private async void PickAnswer(object selectedAnswer)
    {
        if (_timer.IsEnabled)
        {
            _timer.Stop();
            await CheckAnswer(selectedAnswer);
            LoadNextQuestion();
        }
    }
    private async Task CheckAnswer(object selectedAnswer)
    {
        if (selectedAnswer is string answer)
        {
            if (answer == CurrentQuestion.CorrectAnswer)
            {
                await ExecuteVoice("Correct");
                // Icon for correct answer
                //await Task.Delay(1000);
                AmountOfCorrectAnswers++;
            }
            else
            {
                await ExecuteVoice("Wrong answer");
                // Icon for wrong answer
                //await Task.Delay(1000);
            }
        }
    }
    private void RestartGame(object obj) => Start();
    public void Stop()
    {
        _timer.Stop();
        IsPlaying = false;
        MainWindowViewModel.IsPlayMode = false;
    }
}
