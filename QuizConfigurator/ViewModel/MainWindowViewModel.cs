using QuizConfigurator.Commands;
using QuizConfigurator.Model;
using QuizConfigurator.View.Dialogs;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace QuizConfigurator.ViewModel;
public class MainWindowViewModel : BaseViewModel
{
    private DispatcherTimer _timer; // dispatcher.invoke goolge

    private bool _isPlayMode;
    public bool IsPlayMode
    {
        get => _isPlayMode;
        set
        {
            _isPlayMode = value;
            OnPropertyChanged();
        }
    }

    private QuestionPackViewModel? _activePack;
    public QuestionPackViewModel? ActivePack
    {
        get => _activePack;
        set
        {
            _activePack = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ConfigurationViewModel.ActivePack));
        }
    }
    public PlayerViewModel PlayerViewModel { get; }
    public ConfigurationViewModel ConfigurationViewModel { get; }
    public QuestionPackViewModel QuestionPackViewModel { get; }
    public QuestionViewModel QuestionViewModel { get; }
    public QuestionPackViewModel? NewPack { get; private set; }
    public ObservableCollection<QuestionPackViewModel> Packs { get; } = new ObservableCollection<QuestionPackViewModel>();

    public ICommand ToggleFullScreenCommand { get; }
    public ICommand ExitProgramCommand { get; }
    public ICommand SetPlayModeCommand { get; }
    public ICommand SetConfigurationModeCommand { get; }
    public ICommand SetActivePackCommand { get; }
    public ICommand CreateNewPackCommand { get; }
    public ICommand AddNewPackCommand { get; }
    public ICommand RemoveQuestionPackCommand { get; }
    public ICommand ImportQuestionsCommand { get; }

    public MainWindowViewModel()
    {
        ActivePack = new QuestionPackViewModel(new QuestionPack("Default Pack"));

        PlayerViewModel = new PlayerViewModel(this);
        ConfigurationViewModel = new ConfigurationViewModel(this);
        QuestionPackViewModel = new QuestionPackViewModel();
        QuestionViewModel = new QuestionViewModel();


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
        _isPlayMode = false;

        ToggleFullScreenCommand = new RelayCommand(ToggleFullScreen);
        ExitProgramCommand = new RelayCommand(ExitProgram);
        SetPlayModeCommand = new RelayCommand(SetPlayMode, CanSetPlayMode);
        SetConfigurationModeCommand = new RelayCommand(SetConfigurationMode, CanSetConfigurationMode);
        SetActivePackCommand = new RelayCommand(SetActivePack);
        CreateNewPackCommand = new RelayCommand(CreateNewPack);
        AddNewPackCommand = new RelayCommand(AddNewPack);
        RemoveQuestionPackCommand = new RelayCommand(RemoveQuestionPack, CanRemoveQuestionPack);
        ImportQuestionsCommand = new RelayCommand(ImportQuestions, CanImportQuestions);
    }
    private void ToggleFullScreen(object obj)
    {
        var window = Application.Current.MainWindow;
        if (window.WindowState == WindowState.Normal)
        {
            window.WindowState = WindowState.Maximized;
            window.WindowStyle = WindowStyle.None;
            window.Topmost = true;
        }
        else
        {
            window.WindowStyle = WindowStyle.SingleBorderWindow;
            window.WindowState = WindowState.Normal;
            window.Topmost = false;
        }
    }
    private void ExitProgram(object obj)
    {
        var result = MessageBox.Show("Are you sure you want to exit?", "Exit Program",
            MessageBoxButton.YesNo);
        if (result == MessageBoxResult.Yes)
        {
            Application.Current.Shutdown();
        } 
    }
    private bool CanSetPlayMode(object arg) => !_isPlayMode && ActivePack.Questions.Count > 0;
    private void SetPlayMode(object obj)
    {
        IsPlayMode = true;
        PlayerViewModel.Start();
    }
    private bool CanSetConfigurationMode(object arg) => _isPlayMode;
    private void SetConfigurationMode(object obj)
    {
        IsPlayMode = false;
        PlayerViewModel.Stop();
    }
    private void SetActivePack(object obj) => ActivePack = obj as QuestionPackViewModel;
    private void CreateNewPack(object obj)
    {
        var createNewPackDialog = new CreateNewPackDialog();
        if (createNewPackDialog.ShowDialog() == true)
        {
            //Packs.Add(new QuestionPackViewModel(new QuestionPack(createNewPackDialog.PackName)));
        }
        NewPack = new QuestionPackViewModel(new QuestionPack("<PackName>"));
    }
    private void AddNewPack(object obj)
    {
        Packs.Add(NewPack);
        ActivePack = NewPack;
    }
    private bool CanRemoveQuestionPack(object obj) => Packs.Count > 1;
    private void RemoveQuestionPack(object obj) => Packs.Remove(ActivePack);
    private bool CanImportQuestions(object arg) => !_isPlayMode;
    private void ImportQuestions(object obj)
    {
        throw new NotImplementedException();
    }
}
