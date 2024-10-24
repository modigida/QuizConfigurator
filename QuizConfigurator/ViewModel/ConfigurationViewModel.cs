
using QuizConfigurator.Commands;
using QuizConfigurator.Model;
using QuizConfigurator.View.Dialogs;
using System.Collections.ObjectModel;
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
    private QuestionViewModel _activeQuestion;
    public QuestionViewModel ActiveQuestion
    {
        get => _activeQuestion;
        set
        {
            _activeQuestion = value;
            OnPropertyChanged();
        }
    }

    private ObservableCollection<QuestionViewModel> _selectedItems = new ObservableCollection<QuestionViewModel>();
    public ObservableCollection<QuestionViewModel> SelectedItems
    {
        get => _selectedItems;
        set
        {
            _selectedItems = value;
            OnPropertyChanged();
        }
    }
    public ICommand AddQuestionCommand { get; }
    public ICommand RemoveQuestionCommand { get; }
    public ICommand EditPackOptionsCommand { get; }
    public ICommand ClosePackOptionsCommand { get; }
    public ConfigurationViewModel(MainWindowViewModel mainWindowViewModel)
    {
        AddQuestionCommand = new RelayCommand(AddQuestion, CanAddQuestion);
        RemoveQuestionCommand = new RelayCommand(RemoveQuestion, CanRemoveQuestion);
        EditPackOptionsCommand = new RelayCommand(EditPackOptions, CanEditPackOptions);
        ClosePackOptionsCommand = new RelayCommand(ClosePackOptions);
        _mainWindowViewModel = mainWindowViewModel;
    }
    private bool CanAddQuestion(object arg) => !_mainWindowViewModel.IsPlayMode;
    private void AddQuestion(object obj)
    {
        var question = new QuestionViewModel(new Question("New Question", string.Empty, string.Empty, string.Empty, string.Empty));
        _mainWindowViewModel.ActivePack?.Questions.Add(question);
        ActiveQuestion = question;
        SelectedItems.Add(question);
        IsComponentVisible = true;


    }
    private bool CanRemoveQuestion(object arg) => SelectedItems != null && !_mainWindowViewModel.IsPlayMode;
    private void RemoveQuestion(object obj)
    {
        if (SelectedItems != null)
        {
            var questionsToRemove = SelectedItems.ToList();
            foreach (var question in questionsToRemove)
            {
                _mainWindowViewModel.ActivePack?.Questions.Remove(question);
                SelectedItems.Remove(question);
            }
        }
    }
    private bool CanEditPackOptions(object arg) => !_mainWindowViewModel.IsPlayMode;
    private void EditPackOptions(object obj)
    {
        var packOptions = new PackOptionsDialog();
        if (packOptions.ShowDialog() == true)
        {

        }
    }
    private void ClosePackOptions(object obj)
    {
        var window = (Window)obj;
        window.Close();
    }
}
