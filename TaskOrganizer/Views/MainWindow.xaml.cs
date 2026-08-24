using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
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
    /// SelectionChanged remonte (bubbling) depuis n'importe quel Selector des
    /// onglets (ComboBox de tri, de thème, etc.) : on ne réagit qu'à un
    /// changement d'onglet, identifié par e.Source pointant sur le TabControl
    /// lui-même plutôt que sur un contrôle imbriqué.
    /// </summary>
    private void OngletsPrincipaux_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.Source is not TabControl tabControl)
        {
            return;
        }

        tabControl.BeginAnimation(OpacityProperty, new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200)));
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
