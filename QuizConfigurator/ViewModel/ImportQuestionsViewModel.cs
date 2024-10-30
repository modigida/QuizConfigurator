
using Newtonsoft.Json;
using QuizConfigurator.API;
using QuizConfigurator.Commands;
using QuizConfigurator.Model;
using QuizConfigurator.View.Dialogs;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Windows;
using System.Windows.Input;

namespace QuizConfigurator.ViewModel;
public class ImportQuestionsViewModel : BaseViewModel
{
    private MainWindowViewModel _mainWindowViewModel;
    public ImportQuestionsDialog ImportDialog { get; set; }

    public OpenTriviaCategories Category { get; set; }

    private Difficulty _difficulty = Difficulty.Medium;
    public Difficulty Difficulty 
    { get => _difficulty;
        set
        {
            _difficulty = value;
            OnPropertyChanged();
        }
    }
    private int _numberOfQuestions;
    public int NumberOfQuestions 
    { 
        get => _numberOfQuestions;
        set
        {
            _numberOfQuestions = value;
            OnPropertyChanged();
        }
    }
    public ObservableCollection<OpenTriviaCategories> Categories { get; set; }

    public ICommand ExecuteImportCommand { get; }
    public ICommand CancelImportDialogCommand { get; }
    public ImportQuestionsViewModel()
    {
    }
    public ImportQuestionsViewModel(MainWindowViewModel mainWindowViewModel)
    {
        _mainWindowViewModel = mainWindowViewModel;

        Categories = new ObservableCollection<OpenTriviaCategories>();

        LoadCategoriesAsync();
        
        Difficulty = Difficulty.Medium;
        NumberOfQuestions = 5;

        ExecuteImportCommand = new RelayCommand(ExecuteImportQuestions);
        CancelImportDialogCommand = new RelayCommand(mainWindowViewModel.ClosePackDialog);
    }

    public async void OpenImportQuestions(object obj)
    {
        var importDialog = new ImportQuestionsDialog
        {
            Owner = _mainWindowViewModel.ParentWindow,
            DataContext = this
        };

        ImportDialog = importDialog;
        importDialog.ShowDialog();
    }

    private async Task LoadCategoriesAsync()
    {
        using (var client = new HttpClient())
        {
            var url = "https://opentdb.com/api_category.php";
            var response = await client.GetStringAsync(url);

            var result = JsonConvert.DeserializeObject<OpenTriviaResponse>(response);

            foreach (var category in result.trivia_categories)
            {
                Categories.Add(category);
            }

            Category = Categories.FirstOrDefault();
        }
    }
    private async void ExecuteImportQuestions(object obj)
    {
        string amount = NumberOfQuestions.ToString();
        string category = Category.Id.ToString();
        string difficulty = Difficulty.ToString().ToLower();
        await GetTriviaQuestionsAsync(amount, category, difficulty);
        _mainWindowViewModel.ClosePackDialog(ImportDialog);
    }

    public async Task GetTriviaQuestionsAsync(string amount, string category, string difficulty)
    {
        using (var client = new HttpClient())
        {
            try
            {
                string url = @$"https://opentdb.com/api.php?amount={amount}&category={category}&difficulty={difficulty}&type=multiple";
                var response = await client.GetStringAsync(url);

                var result = JsonConvert.DeserializeObject<OpenTriviaResponse>(response);

                foreach (var question in result.results)
                {
                    var newQuestion = new QuestionViewModel(new Question(question.question,
                        question.correct_answer, question.incorrect_answers));
                    _mainWindowViewModel.ActivePack?.Questions.Add(newQuestion);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }

}
