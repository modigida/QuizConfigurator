using QuizConfigurator.ViewModel;
using System.Windows;

namespace QuizConfigurator.View.Dialogs;
public partial class PackOptionsDialog : Window
{
    public PackOptionsDialog()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }
}
