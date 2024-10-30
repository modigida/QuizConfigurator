using QuizConfigurator.ViewModel;
using System.Windows;

namespace QuizConfigurator;
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }
}