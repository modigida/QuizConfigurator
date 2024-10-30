using QuizConfigurator.Commands;
using QuizConfigurator.Model;
using QuizConfigurator.Service;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace QuizConfigurator.ViewModel;
public class ConfigurationViewModel : BaseViewModel
{
    private readonly MainWindowViewModel _mainWindowViewModel;
    private MainWindowViewModelHandlePacks _mainWindowViewModelHandlePacks;

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
                ActiveQuestion = _selectedItems.Count == 1 ? _selectedItems[0] : null;
            }

            OnPropertyChanged();
        }
    }

    private ICommand? _removeQuestionCommand;
    public ICommand? RemoveQuestionCommand 
    {
        get
        {
            return _removeQuestionCommand ??= new RelayCommand(RemoveQuestion, CanRemoveQuestion);
        }
    }
    public ICommand AddQuestionCommand { get; }
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
        _mainWindowViewModel = mainWindowViewModel;
        _mainWindowViewModelHandlePacks = mainWindowViewModel.MainWindowViewModelHandlePacks;

        AddQuestionCommand = new RelayCommand(AddQuestion, CanAddQuestion);
        EditPackOptionsCommand = new RelayCommand(_mainWindowViewModelHandlePacks.EditPackOptions, CanEditPackOptions);
        ClosePackOptionsCommand = new RelayCommand(mainWindowViewModel.ClosePackOptions);

        SelectedItems.CollectionChanged += SelectedItems_CollectionChanged;

        _mainWindowViewModel.PropertyChanged += MainWindowViewModel_PropertyChanged;
    }
    private void MainWindowViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_mainWindowViewModel.ActivePack))
        {
            UpdateIsComponentVisible(); 
        }
    }
    private bool CanAddQuestion(object arg) => !_mainWindowViewModel.IsPlayMode;
    private async void AddQuestion(object obj)
    {
        var question = new QuestionViewModel(new Question("New Question", string.Empty, string.Empty, string.Empty, string.Empty));
        _mainWindowViewModel.ActivePack?.Questions.Add(question);
        
        _mainWindowViewModel.Packs.CollectionChanged += (s, e) => CommandManager.InvalidateRequerySuggested();
        
        ActiveQuestion = question;
        
        SelectedItems.Clear();
        SelectedItems.Add(question);

        await JsonHandler.SavePacksToJson(_mainWindowViewModel);
    }
    private void OnSelectedItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        ActiveQuestion = _selectedItems.Count == 1 ? _selectedItems[0] : null;
    }
    private void SelectedItems_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        ActiveQuestion = SelectedItems.Count == 1 ? SelectedItems.FirstOrDefault() : null;
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
    private bool CanRemoveQuestion(object arg) => SelectedItems.Count > 0 && !_mainWindowViewModel.IsPlayMode;
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
}
