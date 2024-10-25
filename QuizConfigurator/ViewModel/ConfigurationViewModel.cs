using QuizConfigurator.Commands;
using QuizConfigurator.Model;
using QuizConfigurator.View.Dialogs;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Input;

namespace QuizConfigurator.ViewModel;
public class ConfigurationViewModel : BaseViewModel
{
    private readonly MainWindowViewModel _mainWindowViewModel;
    private bool _isComponentVisible;
    public bool IsComponentVisible
    {
        get { return _isComponentVisible; }
        set
        {
            _isComponentVisible = value;
            OnPropertyChanged(nameof(IsComponentVisible));
        }
    }
    public QuestionPackViewModel? ActivePack => _mainWindowViewModel.ActivePack;
    private QuestionViewModel? _activeQuestion;
    public QuestionViewModel? ActiveQuestion
    {
        get => _activeQuestion;
        set
        {
            _activeQuestion = value;
            UpdateIsComponentVisible();
            OnPropertyChanged();
        }
    }

    private ObservableCollection<QuestionViewModel> _selectedItems = new ObservableCollection<QuestionViewModel>();
    public ObservableCollection<QuestionViewModel> SelectedItems
    {
        get => _selectedItems;
        set
        {
            if (_selectedItems != null)
            {
                _selectedItems.CollectionChanged -= SelectedItems_CollectionChanged;
            }

            _selectedItems = value;
            if (_selectedItems != null)
            {
                _selectedItems.CollectionChanged += SelectedItems_CollectionChanged;
            }

            ActiveQuestion = _selectedItems.Count == 1 ? _selectedItems[0] : null;

            OnPropertyChanged();
        }
    }
    
    public ICommand AddQuestionCommand { get; }
    public ICommand RemoveQuestionCommand { get; }
    public ICommand EditPackOptionsCommand { get; }
    public ICommand ClosePackOptionsCommand { get; }

    public string SelectionMessage
    {
        get
        {
            if (SelectedItems.Count == 0)
            {
                return "No question selected";
            }
            else if (SelectedItems.Count > 1)
            {
                return "Several questions selected";
            }
            else
            {
                return string.Empty;
            }
        }
    }
    public ConfigurationViewModel(MainWindowViewModel mainWindowViewModel)
    {
        AddQuestionCommand = new RelayCommand(AddQuestion, CanAddQuestion);
        RemoveQuestionCommand = new RelayCommand(RemoveQuestion, CanRemoveQuestion);
        EditPackOptionsCommand = new RelayCommand(EditPackOptions, CanEditPackOptions);
        ClosePackOptionsCommand = new RelayCommand(mainWindowViewModel.ClosePackOptions);
        _mainWindowViewModel = mainWindowViewModel;
        SelectedItems.CollectionChanged += SelectedItems_CollectionChanged;
    }
    private bool CanAddQuestion(object arg) => !_mainWindowViewModel.IsPlayMode;
    private void AddQuestion(object obj)
    {
        var question = new QuestionViewModel(new Question("New Question", string.Empty, string.Empty, string.Empty, string.Empty));
        _mainWindowViewModel.ActivePack?.Questions.Add(question);
        ActiveQuestion = question;
        SelectedItems.Clear();
        SelectedItems.Add(question);
    }
    private void OnSelectedItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        ActiveQuestion = _selectedItems.Count == 1 ? _selectedItems[0] : null;
    }
    private void SelectedItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        ActiveQuestion = SelectedItems.Count == 1 ? SelectedItems[0] : null;
        UpdateSelectionMessage();
    }
    private void UpdateIsComponentVisible()
    {
        IsComponentVisible = ActiveQuestion != null && SelectedItems.Count == 1;
    }
    private void UpdateSelectionMessage()
    {
        OnPropertyChanged(nameof(SelectionMessage));
    }
    private bool CanRemoveQuestion(object arg) => SelectedItems != null && !_mainWindowViewModel.IsPlayMode;
    private void RemoveQuestion(object obj)
    {
        if (SelectedItems != null)
        {
            MessageBoxResult result;
            var messageOneQuestion = $"Sure to delete {SelectedItems.Count} question?";
            var messageSeveralQuestions = $"Sure to delete {SelectedItems.Count} questions?";
            if (SelectedItems.Count == 1)
            {
                result = MessageBox.Show(messageOneQuestion, "Delete Question", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            }
            else
            {
                result = MessageBox.Show(messageSeveralQuestions, "Delete Question", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            }

            if (result == MessageBoxResult.Yes)
            {
                var questionsToRemove = SelectedItems.ToList();
                foreach (var question in questionsToRemove)
                {
                    _mainWindowViewModel.ActivePack?.Questions.Remove(question);
                    SelectedItems.Remove(question);
                }
            }
        }
    }
    private bool CanEditPackOptions(object arg) => !_mainWindowViewModel.IsPlayMode;
    private void EditPackOptions(object obj)
    {
        _mainWindowViewModel.ButtonToggleContent = "Edit";
        _mainWindowViewModel.CurrentPackCommand = new RelayCommand(SaveEditPackOptions);
        _mainWindowViewModel.UseActivePack = true;
        var packDialog = new PackDialog
        {
            Owner = _mainWindowViewModel.ParentWindow,
            DataContext = _mainWindowViewModel
        };

        if (packDialog.ShowDialog() == true)
        {
            OnPropertyChanged(nameof(_mainWindowViewModel.ActivePack));
            CommandManager.InvalidateRequerySuggested();
        }
    }
    private void SaveEditPackOptions(object obj)
    {
        if (_mainWindowViewModel.ActivePack != null)
        {
            OnPropertyChanged(nameof(_mainWindowViewModel.ActivePack));
        }
        _mainWindowViewModel.ClosePackOptions(obj);
    }
}
