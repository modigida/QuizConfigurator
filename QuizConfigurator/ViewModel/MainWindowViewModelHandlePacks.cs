using QuizConfigurator.Commands;
using QuizConfigurator.Model;
using System.Windows;
using System.Windows.Input;

namespace QuizConfigurator.ViewModel;
public class MainWindowViewModelHandlePacks : BaseViewModel
{
    private MainWindowViewModel _mainWindowViewModel;
    private ConfigurationViewModel _configurationViewModel => _mainWindowViewModel.ConfigurationViewModel;
    public MainWindowViewModelHandlePacks(MainWindowViewModel mainWindowViewModel)
    {
        _mainWindowViewModel = mainWindowViewModel;
    }
    public void SetDefaultActivePack()
    {
        if (_mainWindowViewModel.Packs.Count == 0)
        {
            _mainWindowViewModel.ActivePack = new QuestionPackViewModel(new QuestionPack("Default Pack"));
            _mainWindowViewModel.Packs.Add(_mainWindowViewModel.ActivePack);
        }
        else
        {
            _mainWindowViewModel.ActivePack = _mainWindowViewModel.Packs.FirstOrDefault();
        }
        _mainWindowViewModel.Packs.CollectionChanged += (s, e) => CommandManager.InvalidateRequerySuggested();

    }
    public void SetActivePack(object obj)
    {
        _mainWindowViewModel.ActivePack = obj as QuestionPackViewModel;
        _mainWindowViewModel.ConfigurationViewModel.ActiveQuestion = null;
        _mainWindowViewModel.ConfigurationViewModel.SelectedItems.Clear();
        OnPropertyChanged(nameof(_configurationViewModel.SelectedItems));
        OnPropertyChanged(nameof(_configurationViewModel.ActiveQuestion));
        OnPropertyChanged(nameof(_mainWindowViewModel.ActivePack));
        CommandManager.InvalidateRequerySuggested();
    }

    public void CreateNewPack(object obj)
    {
        _mainWindowViewModel.ButtonToggleContent = "Create";
        _mainWindowViewModel.CurrentPackCommand = new RelayCommand(AddNewPack);
        _mainWindowViewModel.UseActivePack = false;

        _mainWindowViewModel.NewPack = new QuestionPackViewModel(new QuestionPack("<PackName>"));

        var packDialog = _mainWindowViewModel.OpenPackDialog();

        if (packDialog.ShowDialog() == true)
        {
            _mainWindowViewModel.Packs.Add(_mainWindowViewModel.NewPack);
            _mainWindowViewModel.ActivePack = _mainWindowViewModel.NewPack;
            CommandManager.InvalidateRequerySuggested();
        }
    }
    private void AddNewPack(object obj)
    {
        if (_mainWindowViewModel.NewPack != null)
        {
            var newPackViewModel = new QuestionPackViewModel(new QuestionPack(_mainWindowViewModel.NewPack.Name, 
                _mainWindowViewModel.NewPack.Difficulty, _mainWindowViewModel.NewPack.TimeLimitInSeconds));

            _mainWindowViewModel.Packs.Add(newPackViewModel);

            _mainWindowViewModel.ActivePack = newPackViewModel;

            CommandManager.InvalidateRequerySuggested();
            OnPropertyChanged(nameof(_mainWindowViewModel.ActivePack));
        }
        _mainWindowViewModel.ClosePackOptions(obj);
    }
    public void EditPackOptions(object obj)
    {
        _mainWindowViewModel.ButtonToggleContent = "Edit";
        _mainWindowViewModel.CurrentPackCommand = new RelayCommand(SaveEditPackOptions);
        _mainWindowViewModel.UseActivePack = true;

        var packDialog = _mainWindowViewModel.OpenPackDialog();

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
    public void RemoveQuestionPack(object obj)
    {
        if (_mainWindowViewModel.ActivePack != null)
        {
            MessageBoxResult result;
            var messageDeletePack = $"Sure to delete {_mainWindowViewModel.ActivePack.Name}?";
            result = MessageBox.Show(messageDeletePack, "Delete Question", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                _mainWindowViewModel.Packs.Remove(_mainWindowViewModel.ActivePack);
                _mainWindowViewModel.ActivePack = _mainWindowViewModel.Packs.FirstOrDefault();
            }
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
