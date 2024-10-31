using QuizConfigurator.Model;
using System.IO;
using System.Text.Json;
using System.Windows;
using QuizConfigurator.ViewModel;
using System.Collections.ObjectModel;

namespace QuizConfigurator.Service;
public class JsonHandler
{
    public static async Task LoadPacksFromJson(MainWindowViewModel mainWindowViewModel, 
        MainWindowViewModelHandlePacks windowViewModelHandlePacks)
    {
        try
        {
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "packs.json");
            if (File.Exists(filePath))
            {
                string jsonString = await File.ReadAllTextAsync(filePath);

                var loadedQuestionPacks = JsonSerializer.Deserialize<List<QuestionPack>>(jsonString);

                if (loadedQuestionPacks != null)
                {
                    mainWindowViewModel.Packs = new ObservableCollection<QuestionPackViewModel>(
                        loadedQuestionPacks.Select(qp => new QuestionPackViewModel(qp))
                    );

                    mainWindowViewModel.ActivePack = mainWindowViewModel.Packs.FirstOrDefault();
                }
            }
            else
            {
                windowViewModelHandlePacks.SetDefaultActivePack();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading packs: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public static async Task SavePacksToJson(MainWindowViewModel mainWindowViewModel)
    {
        try
        {
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "packs.json");
            string contents = JsonSerializer.Serialize(mainWindowViewModel.Packs);
            await File.WriteAllTextAsync(filePath, contents);

            mainWindowViewModel.CurrentMessageContent = "Packs saved successfully";
            
            await Task.Delay(1000);

            mainWindowViewModel.CurrentMessageContent = string.Empty;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving packs: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
