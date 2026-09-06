/*
 * Task Organizer
 * Copyright (c) 2026 Nicolas Dos Santos
 * Licensed under the MIT License
 */

using System.Windows;
using TaskOrganizer.Models;

namespace TaskOrganizer.Views;

/// <summary>Modal window listing reminders missed while the app was closed.</summary>
public partial class RappelsManquesWindow : Window
{
    public RappelsManquesWindow(IReadOnlyList<Rappel> rappelsManques)
    {
        InitializeComponent();
        DataContext = rappelsManques;
    }

    private void FermerButton_Click(object sender, RoutedEventArgs e) => Close();
}
