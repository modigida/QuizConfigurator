using QuizConfigurator.Commands;
using System.Speech.Synthesis;
using System.Windows.Input;
using System.Windows.Threading;

namespace QuizConfigurator.ViewModel;
public class PlayerViewModel : BaseViewModel
{
    private SpeechSynthesizer _speechSynthesizer;
    public readonly MainWindowViewModel? MainWindowViewModel;

    private DispatcherTimer _timer; 

    private List<QuestionViewModel>? _randomizedQuestions;
    //private List<string>? _randomizedAnswers;
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
    public int AmoundOfQuestions
    {
        get => _randomizedQuestions.Count != null ? _randomizedQuestions.Count : 0;
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
    public PlayerViewModel(MainWindowViewModel? mainWindowViewModel)
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
    public async Task ReadQuestion()
    {
        if (_isSoundOn && CurrentQuestion != null)
        {
            await Task.Run(() => _speechSynthesizer.Speak(CurrentQuestion.Query));
        }
    }
    public void StartQuiz(List<QuestionViewModel> questions)
    {
        InitializeQuestions(questions);
        Start();
    }
    public void Start()
    {
        if(MainWindowViewModel.ActivePack != null)
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
    private void LoadNextQuestion()
    {
        if (CurrentQuestionNumber < _randomizedQuestions.Count)
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
            Task.Delay(15);
            ReadQuestion();
        }
        else
        {
            Stop(); 
        }
    }
    public void StartTimer()
    {
        RemainingTime = MainWindowViewModel.ActivePack.TimeLimitInSeconds; 
        _timer.Start(); 
    }
    private void Timer_Tick(object sender, EventArgs e)
    {
        if (RemainingTime > 0)
        {
            RemainingTime--; 
        }
        else
        {
            _timer.Stop();
            LoadNextQuestion();
        }
    }
    private void PickAnswer(object selectedAnswer)
    {
        if (_timer.IsEnabled)
        {
            _timer.Stop();

            if (selectedAnswer is string answer)
            {
                if (answer == CurrentQuestion.CorrectAnswer)
                {
                    AmountOfCorrectAnswers++;
                }

                // Show Icons for wrong or corrct answer 
                LoadNextQuestion();
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