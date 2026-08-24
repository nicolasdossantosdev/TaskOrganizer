using System.Windows;
using System.Windows.Controls;
using TaskOrganizer.Models;
using TaskOrganizer.ViewModels;

namespace TaskOrganizer.Views;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }

    /// <summary>
    /// Le SelectedItem de la ComboBox est déjà écrit dans Tache.Statut (binding
    /// TwoWay) au moment où cet événement se déclenche ; il ne reste qu'à
    /// persister la tâche via la commande du ViewModel.
    /// </summary>
    private void StatutComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox { DataContext: Tache tache } && DataContext is MainViewModel viewModel)
        {
            viewModel.ChangerStatutCommand.Execute(tache);
        }
    }
}
