using System.Windows;
using TaskOrganizer.ViewModels;

namespace TaskOrganizer.Views;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
