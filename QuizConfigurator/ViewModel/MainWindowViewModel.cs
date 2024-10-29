using QuizConfigurator.Commands;
using QuizConfigurator.Model;
using QuizConfigurator.Service;
using QuizConfigurator.View.Dialogs;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Input;

namespace QuizConfigurator.ViewModel;
public class MainWindowViewModel : BaseViewModel
{
    private QuestionPackViewModel? _newPack;

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
            OnPropertyChanged(nameof(CurrentPack));
            OnPropertyChanged(nameof(UseActivePack));
        }
    }
    
    public Window ParentWindow => Application.Current.MainWindow;
    public QuestionPackViewModel? CurrentPack => _useActivePack ? ActivePack : NewPack;
    public PlayerViewModel PlayerViewModel { get; }
    public ConfigurationViewModel ConfigurationViewModel { get; }
    public QuestionPackViewModel QuestionPackViewModel { get; }
    public QuestionViewModel QuestionViewModel { get; }

    private ObservableCollection<QuestionPackViewModel> _packs;
    public ObservableCollection<QuestionPackViewModel> Packs
    {
        get => _packs;
        set
        {
            _packs = value;
            OnPropertyChanged();
            CommandManager.InvalidateRequerySuggested();
        }
    }

    public ICommand ToggleFullScreenCommand { get; }
    public ICommand ExitProgramCommand { get; }
    public ICommand SetPlayModeCommand { get; }
    public ICommand SetConfigurationModeCommand { get; }
    public ICommand SetActivePackCommand { get; }
    public ICommand CreateNewPackCommand { get; }
    public ICommand CurrentPackCommand { get; set; }

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
        Packs = new ObservableCollection<QuestionPackViewModel>();

        LoadPacksAsync();

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
        ImportQuestionsCommand = new RelayCommand(ImportQuestions, CanImportQuestions);

        Packs.CollectionChanged += Packs_CollectionChanged;
    }

    private async void LoadPacksAsync()
    {
        await JsonHandler.LoadPacksFromJson(this);
    }
    private async void Packs_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        await JsonHandler.SavePacksToJson(this);
    }
    public void SetDefaultActivePack()
    {
        if (Packs.Count == 0)
        {
            ActivePack = new QuestionPackViewModel(new QuestionPack("Default Pack"));
            Packs.Add(ActivePack);
        }
        else
        {
            ActivePack = Packs.FirstOrDefault();
        }
        Packs.CollectionChanged += (s, e) => CommandManager.InvalidateRequerySuggested();
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
    }
    private void SetActivePack(object obj)
    {
        ActivePack = obj as QuestionPackViewModel;
        ConfigurationViewModel.ActiveQuestion = null;
        ConfigurationViewModel.SelectedItems.Clear();
        OnPropertyChanged(nameof(ConfigurationViewModel.SelectedItems));
        OnPropertyChanged(nameof(ConfigurationViewModel.ActiveQuestion));
        OnPropertyChanged(nameof(ActivePack));
        CommandManager.InvalidateRequerySuggested();
    }
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

            OnPropertyChanged(nameof(ActivePack));
        }
        ClosePackOptions(obj);
    }
    public void ClosePackOptions(object obj)
    {
        var window = (Window)obj;
        window.Close();
    }
    private bool CanRemoveQuestionPack(object obj) => Packs.Count > 1 && !IsPlayMode;
    private void RemoveQuestionPack(object obj)
    {
        if (ActivePack != null)
        {
            MessageBoxResult result;
            var messageDeletePack = $"Sure to delete {ActivePack.Name}?";
            result = MessageBox.Show(messageDeletePack, "Delete Question", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                Packs.Remove(ActivePack);
                ActivePack = Packs.FirstOrDefault();
            }
            CommandManager.InvalidateRequerySuggested();
        }
    }
    private bool CanImportQuestions(object arg) => !_isPlayMode;
    private void ImportQuestions(object obj)
    {
        throw new NotImplementedException();
    }

    private async void ExitProgram(object obj)
    {
        var result = MessageBox.Show("Are you sure you want to exit?", "Exit Program",
            MessageBoxButton.YesNo);
        if (result == MessageBoxResult.Yes)
        {
            await JsonHandler.SavePacksToJson(this);
            Application.Current.Shutdown();
        }
    }
}
