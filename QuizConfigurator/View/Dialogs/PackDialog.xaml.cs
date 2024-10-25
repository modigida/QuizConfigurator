using QuizConfigurator.ViewModel;
using System.Windows;

namespace QuizConfigurator.View.Dialogs;
public partial class PackDialog : Window
{
    public PackDialog()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }
}
