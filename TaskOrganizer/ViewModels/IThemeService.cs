/*
 * Task Organizer
 * Copyright (c) 2026 Nicolas Dos Santos
 * Licensed under the MIT License
 */

using TaskOrganizer.Models;

namespace TaskOrganizer.ViewModels;

/// <summary>
/// Applies the light/dark/system theme to the application. Defined here (like
/// <see cref="IDialogService"/>) but implemented in Views since it manipulates
/// WPF (<c>Application.ThemeMode</c>); keeps <see cref="ParametresViewModel"/>
/// testable without depending on WPF.
/// </summary>
public interface IThemeService
{
    void Appliquer(ThemeApplication theme);
}
