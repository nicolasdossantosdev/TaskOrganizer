/*
 * Task Organizer
 * Copyright (c) 2026 Nicolas Dos Santos
 * Licensed under the MIT License
 */

using System.Windows;
using TaskOrganizer.Models;

namespace TaskOrganizer.Views;

public partial class RappelsManquesWindow : Window
{
    public RappelsManquesWindow(IReadOnlyList<Rappel> rappelsManques)
    {
        InitializeComponent();
        DataContext = rappelsManques;
    }

    private void FermerButton_Click(object sender, RoutedEventArgs e) => Close();
}
