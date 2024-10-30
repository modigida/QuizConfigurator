using QuizConfigurator.API;
using QuizConfigurator.Commands;
using QuizConfigurator.Service;
using QuizConfigurator.View.Dialogs;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Net.Http;
using System.Windows;
using System.Windows.Input;
using Newtonsoft.Json;
using QuizConfigurator.Model;
using Application = System.Windows.Application;

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
            CommandManager.InvalidateRequerySuggested();
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
    public PlayerViewModelCheckAnswer PlayerViewModelCheckAnswer { get; }
    public ConfigurationViewModel ConfigurationViewModel { get; }
    public QuestionPackViewModel QuestionPackViewModel { get; }
    public QuestionViewModel QuestionViewModel { get; }
    public PlayerViewModelTimer PlayerViewModelTickingSound { get; }
    public MainWindowViewModelHandlePacks MainWindowViewModelHandlePacks { get; }
    public ImportQuestionsViewModel ImportQuestionsViewModel { get; }

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
    public ICommand SaveToJsonCommand { get; set; }

    private ICommand _removeQuestionPackCommand;
    public ICommand RemoveQuestionPackCommand
    {
        get
        {
            return _removeQuestionPackCommand ??= new RelayCommand(MainWindowViewModelHandlePacks.RemoveQuestionPack, 
                CanRemoveQuestionPack);
        }
    }
    public ICommand OpenImportQuestionsCommand { get; }

    public MainWindowViewModel()
    {
        Packs = new ObservableCollection<QuestionPackViewModel>();

        LoadPacksAsync();

        MainWindowViewModelHandlePacks = new MainWindowViewModelHandlePacks(this);
        PlayerViewModelCheckAnswer = new PlayerViewModelCheckAnswer();
        PlayerViewModelTickingSound = new PlayerViewModelTimer(this);
        PlayerViewModel = new PlayerViewModel(this, PlayerViewModelCheckAnswer, PlayerViewModelTickingSound);

        ConfigurationViewModel = new ConfigurationViewModel(this);
        QuestionPackViewModel = new QuestionPackViewModel();
        QuestionViewModel = new QuestionViewModel();

        ImportQuestionsViewModel = new ImportQuestionsViewModel(this);

        _isPlayMode = false;

        ToggleFullScreenCommand = new RelayCommand(ToggleFullScreen);
        ExitProgramCommand = new RelayCommand(ExitProgram);
        SetPlayModeCommand = new RelayCommand(SetPlayMode, CanSetPlayMode);
        SetConfigurationModeCommand = new RelayCommand(SetConfigurationMode, CanSetConfigurationMode);
        SetActivePackCommand = new RelayCommand(MainWindowViewModelHandlePacks.SetActivePack);
        CreateNewPackCommand = new RelayCommand(MainWindowViewModelHandlePacks.CreateNewPack);
        OpenImportQuestionsCommand = new RelayCommand(ImportQuestionsViewModel.OpenImportQuestions, CanImportQuestions);
        SaveToJsonCommand = new RelayCommand(SaveToJson);
        
        Packs.CollectionChanged += Packs_CollectionChanged;
    }

    private async void SaveToJson(object obj)
    {
        await JsonHandler.SavePacksToJson(this);
    }

    private async void LoadPacksAsync()
    {
        await JsonHandler.LoadPacksFromJson(this, MainWindowViewModelHandlePacks);
    }
    private async void Packs_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        await JsonHandler.SavePacksToJson(this);
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
    public PackDialog OpenPackDialog()
    {
        var packDialog = new PackDialog
        {
            Owner = ParentWindow,
            DataContext = this
        };
        return packDialog;
    }

    private bool CanSetPlayMode(object arg) => !_isPlayMode && ActivePack.Questions.Count > 0;
    private async void SetPlayMode(object obj)
    {
        IsPlayMode = true;
        await PlayerViewModel.Start();
    }
    private bool CanSetConfigurationMode(object arg) => _isPlayMode;
    private void SetConfigurationMode(object obj)
    {
        IsPlayMode = false;
        PlayerViewModelTickingSound.Timer.Stop();
    }
    private bool CanRemoveQuestionPack(object obj) => Packs.Count > 1 && !IsPlayMode;
    public void ClosePackDialog(object obj)
    {
        var window = (Window)obj;
        window.Close();
    }
    private bool CanImportQuestions(object arg) => !_isPlayMode && ActivePack != null;

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
