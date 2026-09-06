/*
 * Task Organizer
 * Copyright (c) 2026 Nicolas Dos Santos
 * Licensed under the MIT License
 */

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using TaskOrganizer.Models;
using TaskOrganizer.ViewModels;

namespace TaskOrganizer.Views;

/// <summary>Application main window: task list, Planning and Settings tabs.</summary>
public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }

    /// <summary>
    /// SelectionChanged bubbles up from any Selector inside the tabs (sort
    /// ComboBox, theme ComboBox, etc.): only react to an actual tab change,
    /// identified by e.Source pointing at the TabControl itself rather than a
    /// nested control.
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
    /// The ComboBox's SelectedItem is already written to Tache.Statut (TwoWay
    /// binding) by the time this event fires; all that's left is to persist
    /// the task via the ViewModel's command.
    /// </summary>
    private void StatutComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox { DataContext: Tache tache } && DataContext is MainViewModel viewModel)
        {
            viewModel.ChangerStatutCommand.Execute(tache);
        }
    }
}
