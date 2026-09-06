/*
 * Task Organizer
 * Copyright (c) 2026 Nicolas Dos Santos
 * Licensed under the MIT License
 */

using System.Windows;
using TaskOrganizer.ViewModels;

namespace TaskOrganizer.Views;

/// <summary>Modal window hosting the task edit form.</summary>
public partial class EditTacheWindow : Window
{
    public EditTacheWindow(EditTacheViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        viewModel.TacheEnregistree += (_, _) =>
        {
            DialogResult = true;
            Close();
        };
    }

    private void AnnulerButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
