﻿using QuizConfigurator.ViewModel;
using System.Windows;

namespace QuizConfigurator.View.Dialogs;
public partial class ImportQuestionsDialog : Window
{
    public ImportQuestionsDialog()
    {
        InitializeComponent();
        DataContext = new ImportQuestionsViewModel();
    }
}
