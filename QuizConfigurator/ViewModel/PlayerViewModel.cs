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

    private int _correctAnswerIndex;
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

    private int _currentQuestionNumber;
    public int CurrentQuestionNumber
    {
        get => _currentQuestionNumber;
        set
        {
            _currentQuestionNumber = value;
            OnPropertyChanged();
            CurrentQuestionInOrder = $"Question {_currentQuestionNumber} of {_amountOfQuestions}"; 
        }
    }
    private string _resultString;
    public string ResultString
    {
        get => $"You got {AmountOfCorrectAnswers} out of {AmountOfQuestions} answers correct!";
        set
        {
            _resultString = value;
            OnPropertyChanged();
            
        }
    }

    private int _amountOfQuestions;
    public int AmountOfQuestions
    {
        get => _randomizedQuestions?.Count ?? 0;
        set
        {
            _amountOfQuestions = value;
            OnPropertyChanged();
        }
    }

    private string _currentQuestionNumberInOrder;
    public string CurrentQuestionInOrder
    {
        get => $"Question {CurrentQuestionNumber} of {AmountOfQuestions}";
        private set 
        {
            _currentQuestionNumberInOrder = value;
            OnPropertyChanged();
        }
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
    private bool _isGameOver;
    public bool IsGameOver
    {
        get => _isGameOver;
        set
        {
            _isGameOver = value;
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

    private bool _isAnswerCorrect;
    public bool IsAnswerCorrect
    {
        get => _isAnswerCorrect;
        set
        {
            _isAnswerCorrect = value;
            OnPropertyChanged();
            
        }
    }

    private int _amountOfCorrectAnswers;
    public int AmountOfCorrectAnswers
    {
        get => _amountOfCorrectAnswers;

        set
        {
            _amountOfCorrectAnswers = value;
            OnPropertyChanged();
            ResultString = $"You got {AmountOfCorrectAnswers} out of {_amountOfQuestions} answers correct";
        }
    }

    private Visibility _correctImageVisibility;
    public Visibility CorrectImageVisibility
    {
        get => _correctImageVisibility;
        set
        {
            _correctImageVisibility = value;
            OnPropertyChanged();
        }
    }

    public ICommand PickAnswerCommand { get; }
    public ICommand RestartGameCommand { get; }
    public ICommand SetSoundSettingCommand { get; }

    public bool IsVisibleCorrectIconAnswerZero { get; private set; }
    public bool IsVisibleIncorrectIconAnswerZero { get; private set; }
    public bool IsVisibleCorrectIconAnswerOne { get; private set; }
    public bool IsVisibleIncorrectIconAnswerOne { get; private set; }
    public bool IsVisibleCorrectIconAnswerTwo { get; private set; }
    public bool IsVisibleIncorrectIconAnswerTwo { get; private set; }
    public bool IsVisibleCorrectIconAnswerThree { get; private set; }
    public bool IsVisibleIncorrectIconAnswerThree { get; private set; }

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
        IsGameOver = false;

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
            IsGameOver = true;
            _timer.Stop();
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
    private void Timer_Tick(object? sender, EventArgs e)
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
            IsAnswerCorrect = answer == CurrentQuestion.CorrectAnswer;
            int selectedIndex = CurrentAnswerOptions.IndexOf(answer);
            int correctIndex = CurrentAnswerOptions.IndexOf(CurrentQuestion.CorrectAnswer);

            HideAnswerIcons();

            DisplayCorrectAnswerIcon(correctIndex);
            DisplaySelectedIncorrectAnswerIcon(selectedIndex);

            await ExecuteVoice(IsAnswerCorrect ? "Correct" : "Wrong answer");

            if (IsAnswerCorrect)
            {
                AmountOfCorrectAnswers++;
            }

            UpdateIconVisibility();

            await Task.Delay(1000);

            HideAnswerIcons();
            
            UpdateIconVisibility();
        }
    }

    private void UpdateIconVisibility()
    {
        OnPropertyChanged(nameof(IsVisibleCorrectIconAnswerZero));
        OnPropertyChanged(nameof(IsVisibleIncorrectIconAnswerZero));
        OnPropertyChanged(nameof(IsVisibleCorrectIconAnswerOne));
        OnPropertyChanged(nameof(IsVisibleIncorrectIconAnswerOne));
        OnPropertyChanged(nameof(IsVisibleCorrectIconAnswerTwo));
        OnPropertyChanged(nameof(IsVisibleIncorrectIconAnswerTwo));
        OnPropertyChanged(nameof(IsVisibleCorrectIconAnswerThree));
        OnPropertyChanged(nameof(IsVisibleIncorrectIconAnswerThree));
    }

    private void DisplayCorrectAnswerIcon(int correctIndex)
    {
        switch (correctIndex)
        {
            case 0:
                IsVisibleCorrectIconAnswerZero = true;
                break;
            case 1:
                IsVisibleCorrectIconAnswerOne = true;
                break;
            case 2:
                IsVisibleCorrectIconAnswerTwo = true;
                break;
            case 3:
                IsVisibleCorrectIconAnswerThree = true;
                break;
        }
    }
    private void DisplaySelectedIncorrectAnswerIcon(int selectedIndex)
    {
        if (!IsAnswerCorrect)
        {
            switch (selectedIndex)
            {
                case 0:
                    IsVisibleIncorrectIconAnswerZero = true;
                    break;
                case 1:
                    IsVisibleIncorrectIconAnswerOne = true;
                    break;
                case 2:
                    IsVisibleIncorrectIconAnswerTwo = true;
                    break;
                case 3:
                    IsVisibleIncorrectIconAnswerThree = true;
                    break;
            }
        }
    }
    private void HideAnswerIcons()
    {
        IsVisibleCorrectIconAnswerZero = false;
        IsVisibleIncorrectIconAnswerZero = false;
        IsVisibleCorrectIconAnswerOne = false;
        IsVisibleIncorrectIconAnswerOne = false;
        IsVisibleCorrectIconAnswerTwo = false;
        IsVisibleIncorrectIconAnswerTwo = false;
        IsVisibleCorrectIconAnswerThree = false;
        IsVisibleIncorrectIconAnswerThree = false;
    }

    private void RestartGame(object obj) => Start();
}
