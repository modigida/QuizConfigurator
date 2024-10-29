using QuizConfigurator.Model;
using System;
using System.IO;
using System.Text.Json;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;
using System.Windows;
using QuizConfigurator.ViewModel;
using System.Collections.ObjectModel;

namespace QuizConfigurator.Service;
public class JsonHandler
{
    public async Task LoadPacksFromJson(MainWindowViewModel mainWindowViewModel)
    {
        try
        {
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "packs.json");
            if (File.Exists(filePath))
            {
                string jsonString = File.ReadAllText(filePath);

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
                mainWindowViewModel.SetDefaultActivePack();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading packs: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public async Task SavePacksToJson(MainWindowViewModel mainWindowViewModel)
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            // Konverterar listan av QuestionPackViewModel till deras model-objekt
            var questionPacks = mainWindowViewModel.Packs.Select(p => p.Model).ToList();

            // Serialiserar till JSON-format
            string jsonString = JsonSerializer.Serialize(questionPacks, options);
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "packs.json");

            // Använder WriteAllTextAsync för asynkron filskrivning
            await File.WriteAllTextAsync(filePath, jsonString);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving packs: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
