using QuizConfigurator.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace QuizConfigurator.View.Dialogs;
public partial class CreateNewPackDialog : Window
{
    public CreateNewPackDialog()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }
}
