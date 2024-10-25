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
    public QuestionPackViewModel? NewPack
    {
        get => _newPack;
        set
        {
            _newPack = value;
            OnPropertyChanged();
        }
    }
    private string _buttonToggleContent;
    public string ButtonToggleContent
    {
        get => _buttonToggleContent;
        set
        {
            _buttonToggleContent = value;
            OnPropertyChanged(nameof(ButtonToggleContent));
        }
    }
    private bool _useActivePack;
    public bool UseActivePack
    {
        get => _useActivePack;
        set
        {
            _useActivePack = value;
            OnPropertyChanged(nameof(CurrentPack)); // Notify that CurrentPack has changed
            OnPropertyChanged(nameof(UseActivePack)); // Notify for the UI to reflect changes
        }
    }
    public Window ParentWindow => Application.Current.MainWindow;
    public QuestionPackViewModel CurrentPack => _useActivePack ? ActivePack : NewPack;
    public PlayerViewModel PlayerViewModel { get; }
    public ConfigurationViewModel ConfigurationViewModel { get; }
    public QuestionPackViewModel QuestionPackViewModel { get; }
    public QuestionViewModel QuestionViewModel { get; }

    private QuestionPackViewModel? _newPack;
    public ObservableCollection<QuestionPackViewModel> Packs { get; } = new ObservableCollection<QuestionPackViewModel>();

    public ICommand ToggleFullScreenCommand { get; }
    public ICommand ExitProgramCommand { get; }
    public ICommand SetPlayModeCommand { get; }
    public ICommand SetConfigurationModeCommand { get; }
    public ICommand SetActivePackCommand { get; }
    public ICommand CreateNewPackCommand { get; }
    public ICommand CurrentPackCommand { get; set; }
    //public ICommand RemoveQuestionPackCommand { get; }
    private ICommand _removeQuestionPackCommand;
    public ICommand RemoveQuestionPackCommand
    {
        get
        {
            return _removeQuestionPackCommand ??= new RelayCommand(RemoveQuestionPack, CanRemoveQuestionPack);
        }
    }
    public ICommand ImportQuestionsCommand { get; }

    public MainWindowViewModel()
    {
        ActivePack = new QuestionPackViewModel(new QuestionPack("Default Pack"));
        Packs.Add(ActivePack);

        PlayerViewModel = new PlayerViewModel(this);
        ConfigurationViewModel = new ConfigurationViewModel(this);
        QuestionPackViewModel = new QuestionPackViewModel();
        QuestionViewModel = new QuestionViewModel();
        
        _isPlayMode = false;

        ToggleFullScreenCommand = new RelayCommand(ToggleFullScreen);
        ExitProgramCommand = new RelayCommand(ExitProgram);
        SetPlayModeCommand = new RelayCommand(SetPlayMode, CanSetPlayMode);
        SetConfigurationModeCommand = new RelayCommand(SetConfigurationMode, CanSetConfigurationMode);
        SetActivePackCommand = new RelayCommand(SetActivePack);
        CreateNewPackCommand = new RelayCommand(CreateNewPack);
        //RemoveQuestionPackCommand = new RelayCommand(RemoveQuestionPack, CanRemoveQuestionPack);
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
        ButtonToggleContent = "Create";
        CurrentPackCommand = new RelayCommand(AddNewPack);
        _useActivePack = false;

        NewPack = new QuestionPackViewModel(new QuestionPack("<PackName>"));

        var packDialog = new PackDialog
        {
            Owner = ParentWindow,
            DataContext = this
        };

        if (packDialog.ShowDialog() == true)
        {
            Packs.Add(NewPack);
            ActivePack = NewPack;
            CommandManager.InvalidateRequerySuggested();
        }
    }
    private void AddNewPack(object obj)
    {
        if (NewPack != null)
        {
            var newPackViewModel = new QuestionPackViewModel(new QuestionPack(NewPack.Name, NewPack.Difficulty, NewPack.TimeLimitInSeconds));

            Packs.Add(newPackViewModel);

            ActivePack = newPackViewModel;

            CommandManager.InvalidateRequerySuggested();
        }
        ClosePackOptions(obj);
    }
    public void ClosePackOptions(object obj)
    {
        var window = (Window)obj;
        window.Close();
    }
    private bool CanRemoveQuestionPack(object obj) => Packs.Count > 1;
    //private void RemoveQuestionPack(object obj) => Packs.Remove(ActivePack);
    private void RemoveQuestionPack(object obj)
    {
        if (ActivePack != null)
        {
            Packs.Remove(ActivePack);
            ActivePack = Packs.FirstOrDefault();

            CommandManager.InvalidateRequerySuggested();
        }
    }
    private bool CanImportQuestions(object arg) => !_isPlayMode;
    private void ImportQuestions(object obj)
    {
        throw new NotImplementedException();
    }
}
