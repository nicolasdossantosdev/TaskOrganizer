/*
 * Task Organizer
 * Copyright (c) 2026 Nicolas Dos Santos
 * Licensed under the MIT License
 */

using System.Windows;
using TaskOrganizer.Models;
using TaskOrganizer.ViewModels;

namespace TaskOrganizer.Infrastructure;

/// <summary>
/// Applies the selected application theme via WPF's <c>ThemeMode</c>.
/// </summary>
public class ThemeService : IThemeService
{
    public void Appliquer(ThemeApplication theme)
    {
        Application.Current.ThemeMode = theme switch
        {
            ThemeApplication.Clair => ThemeMode.Light,
            ThemeApplication.Sombre => ThemeMode.Dark,
            _ => ThemeMode.System,
        };
    }
}
